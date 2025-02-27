/*****************************************************************************
// File Name :         ProceduralVine.cs
// Author :            Tommy Roberts
// Creation Date :     1/31/2025
//
// Brief Description : This script controls the procedural "room guarding movement" functionality
//                     as well as the procedural "lunge attack"
*****************************************************************************/
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using PathCreation;
using DG.Tweening;
using UnityEngine.Animations.Rigging;

/// <summary>
/// Controls all functionality for the attack procedural animation and room movement
/// </summary>
public class ProceduralVine : MonoBehaviour
{
    [Header("Path Stuff")]
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private float _speed = 5f; // speed of idle path following movement
    private float _idleMoveDistance = 0; //current distance along the path

    [Header("Attack stuff")]
    [SerializeField] private Transform _flowerHeadTransform;
    [SerializeField] private Transform _followTransform;
    [SerializeField] private ChainIKConstraint _chainIK;
    [SerializeField] private Rig _dampedTransformRig;
    [SerializeField] private Rig _chainIKRig;
    [SerializeField] private RigBuilder _rigBuilder;
    [SerializeField] private float _rearBackTime = 0.3f;
    [SerializeField] private float _rearBackDistance = 1f;
    [SerializeField] private float _waitAfterRearBackTime = .5f;
    [SerializeField] private float _lungeToPlayerDuration = .2f;
    [SerializeField] private float _moveBackAfterAttackTime = 2f;

    [Header("Attack CD")]
    [Tooltip("after the vine gets back to the path post attacking this is how long it waits before triggers another")]
    [SerializeField] private float _cooldownToAttackAfterMoveBackToPath = .2f; // after the vine gets back to the path post attacking this is how long it waits before triggers another
    private float _currentAttackCD;

    [Header("Retract stuff")]
    [SerializeField] private PathCreator _retractPath; // The target to move toward
    [SerializeField] private Transform _baseOfVine;
    [SerializeField] private float _retractSpeed = 5f; // Speed of movement
    [SerializeField] private float _retractDistance = 0;

    [Header("Appear Stuff")]
    [SerializeField] private PathCreator _appearPath; // The target to move toward
    [SerializeField] private float _appearSpeed = 5f; // Speed of movement
    [SerializeField] private float _appearDistance = 0;
    [SerializeField] private bool _isAppeared = false;

    [SerializeField] private float _moveBackToPathDuration = .3f;

    //state stuff
    public enum EVineState
    {
        attacking,
        retracting,
        appearing,
        shifting,
        none
    }

    private EVineState _currentState;

    private EventInstance _movementEventInstance;

    private void Start()
    {
        CreateMovementAudio();
        StartMovementAudio();
        _currentState = EVineState.none;
    }

    /// <summary>
    /// Updates the vine movement only right now
    /// </summary>
    private void Update() 
    {
        if(_currentState == EVineState.none)
        {
            //move along path
            MoveAlongPath();
            if(_currentAttackCD > 0)
            {
               _currentAttackCD -= Time.deltaTime; 
            }    
        }

        //retracting
        if(_currentState == EVineState.retracting && _retractPath.path.length > _retractDistance +.1f)
        {
            Retracting();
        }
        else if(_baseOfVine.parent.gameObject.activeInHierarchy && _currentState == EVineState.retracting)
        {
            _currentState = EVineState.none;
        }

        //appearing
        if (_currentState == EVineState.appearing && _appearPath.path.length > _appearDistance +.1f)
        {
            Appearing();
        }
        else if (_currentState == EVineState.appearing)
        {
            StartCoroutine(JumpBackToPath(_moveBackToPathDuration));
        }
    }

    /// <summary>
    /// After the vine appears fully, this function makes the head lean down toward the center of the path
    /// </summary>
    /// <param name="timeToGetToPath"></param>
    /// <returns></returns>
    private IEnumerator JumpBackToPath(float timeToGetToPath)
    {
        _currentState = EVineState.shifting;
        yield return new WaitForSeconds(.5f);
        //change rig to use chainIK
        _dampedTransformRig.weight = 0f;
        _chainIKRig.weight = 1f;
        _followTransform.position = _flowerHeadTransform.position;
        _rigBuilder.Build();
        _idleMoveDistance = 0;
        _followTransform.DOJump(_pathCreator.path.GetPointAtDistance(_idleMoveDistance), .2f, 1, timeToGetToPath, false).SetEase(Ease.OutQuad);
        Vector3 direction = (_pathCreator.path.GetPointAtDistance(_idleMoveDistance) - _followTransform.position).normalized;
        _followTransform.forward = direction;
        yield return new WaitForSeconds(timeToGetToPath);
        _currentState = EVineState.none;
        _isAppeared = true;
    }

    /// <summary>
    /// Moves the vine along the given path
    /// </summary>
    private void MoveAlongPath()
    {
        _idleMoveDistance += Time.deltaTime * _speed;
        _followTransform.position = _pathCreator.path.GetPointAtDistance(_idleMoveDistance);
    }

    /// <summary>
    /// Creates the initial instance of the movement audio
    /// </summary>
    private void CreateMovementAudio()
    {
        //return statement added so as not to throw a thousand nulls in logs
        if(RuntimeSfxManager.Instance == null || FmodSfxEvents.Instance == null)
        {
            return;
        }

        _movementEventInstance = RuntimeSfxManager.Instance.
            CreateInstanceFromReference(FmodSfxEvents.Instance.LimbMove, _flowerHeadTransform.gameObject);
    }

    /// <summary>
    /// Starts playing the movement audio
    /// </summary>
    private void StartMovementAudio()
    {
        if(!_movementEventInstance.isValid())
        {
            return;
        }
        RuntimeSfxManager.Instance.FadeInLoopingOneShot(_movementEventInstance, FmodSfxEvents.Instance.LimbMoveFadeInTime);
    }

    /// <summary>
    /// Stops playing the movement audio
    /// </summary>
    private void StopMovementAudio()
    {
        if (!_movementEventInstance.isValid())
        {
            return;
        }
        RuntimeSfxManager.Instance.FadeOutLoopingOneShot(_movementEventInstance, FmodSfxEvents.Instance.LimbMoveFadeOutTime);
    }

    /// <summary>
    /// does the movement for the wind up and lunge attack
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    private IEnumerator Attack(Vector3 playerPosition)
    {
        _currentState = EVineState.attacking;
        _currentAttackCD = _cooldownToAttackAfterMoveBackToPath;
        StopMovementAudio();

        // Calculate the direction from the current object to the target object (X and Z only)
        Vector3 direction = (playerPosition - _followTransform.position).normalized;
        _followTransform.forward = direction;

        //rear back and raise up a little
        Vector3 posToRearBack = _followTransform.position + -direction * _rearBackDistance;
        _followTransform.DOJump(posToRearBack, -.5f, 1, _rearBackTime, false).SetEase(Ease.InSine);
        yield return new WaitForSeconds(_rearBackTime);
        posToRearBack = _followTransform.position + direction * _rearBackDistance;
        _followTransform.DOMove(posToRearBack, _rearBackTime, false).SetEase(Ease.OutSine);
        yield return new WaitForSeconds(_rearBackTime + _waitAfterRearBackTime);

        //redo direction from new position
        direction = (playerPosition - _followTransform.position).normalized;
        var strikePos = _followTransform.position + direction * Vector3.Distance(_followTransform.position, playerPosition) + Vector3.up * .3f;

        //snaps to player
        _followTransform.DOMove(strikePos, _lungeToPlayerDuration, false).SetEase(Ease.OutBack);
        //Plays attack audio
        RuntimeSfxManager.APlayOneShotSfxAttached(FmodSfxEvents.Instance.LimbAttack, _flowerHeadTransform.gameObject);
        yield return new WaitForSeconds(_lungeToPlayerDuration);

        //move back to og position
        _followTransform.DOJump(_pathCreator.path.GetPointAtDistance(_idleMoveDistance), .2f, 1, _moveBackAfterAttackTime, false).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(_moveBackAfterAttackTime);

        // //attack done
        _currentState = EVineState.none;
        StartMovementAudio();
    }

    /// <summary>
    /// the original way we detected if the player was entering the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerStay(Collider collider)
    {
        if(IsColliderPlayer(collider) && _currentState != EVineState.attacking && _currentAttackCD <= 0 && _isAppeared)
        {
            //start attack
            StartCoroutine(Attack(collider.transform.position));
        }
    }

    /// <summary>
    /// Checks for player collision script on a the gameobject of a given collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    private bool IsColliderPlayer(Collider collider)
    {
        return collider.gameObject.GetComponent<PlayerCollision>();
    }

    #region Retract

    /// <summary>
    /// starts the vine retract
    /// </summary>
    public void StartRetract()
    {
        //shift the rig to use the dampedTransform rather than the chainIK
        _retractDistance = 0;
        _currentState = EVineState.retracting;
        _chainIKRig.weight = 0;
        _dampedTransformRig.weight = 1;
    }

    /// <summary>
    /// retracts the vine
    /// </summary>
    private void Retracting()
    {
        //makes base of vine move down the retract path
        _retractDistance += Time.deltaTime * _retractSpeed;
        _baseOfVine.position = _retractPath.path.GetPointAtDistance(_retractDistance);
        _baseOfVine.right = _retractPath.path.GetDirectionAtDistance(_retractDistance);
    }
    #endregion


    #region Appear

    /// <summary>
    /// starts the vine appear
    /// </summary>
    public void StartAppear()
    {
        //skip if already appearing or appeared
        if(_currentState == EVineState.appearing || _isAppeared)
        {
            return;
        }

        //shift the rig to used the dampedTransform instead of IK
        _appearDistance = 0;
        _currentState = EVineState.appearing;
        _chainIKRig.weight = 0;
        _dampedTransformRig.weight = 1;
        _rigBuilder.Build();
        _baseOfVine.position = _appearPath.path.GetPointAtDistance(0);
    }

    /// <summary>
    ///  the vine appears!
    /// </summary>
    private void Appearing()
    {
        //makes the base of the vine start moving up, cannot make system follow the head(it doesn't work)
        _appearDistance += Time.deltaTime * _appearSpeed;
        _baseOfVine.position = _appearPath.path.GetPointAtDistance(_appearDistance);
        _baseOfVine.right = -_appearPath.path.GetDirectionAtDistance(_appearDistance);
    }
    #endregion


    public bool GetIsAppeared() => _isAppeared;
    public EVineState GetVineState() => _currentState;
}
