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
    private float _distance = 0; //current distance along the path
    private bool _isAttacking = false;

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

    [Header("Retract stuff")]
    [SerializeField] private PathCreator _retractPath; // The target to move toward
    [SerializeField] private Transform _baseOfVine;
    [SerializeField] private float _retractSpeed = 5f; // Speed of movement
    [SerializeField] private float _retractDistance = 0;

    private bool _isRetracting = false;

    private EventInstance _movementEventInstance;

    private void Start()
    {
        CreateMovementAudio();
        StartMovementAudio();
    }

    /// <summary>
    /// Updates the vine movement only right now
    /// </summary>
    private void Update() 
    {
        if(!_isAttacking)
        if(!_isAttacking && !_isRetracting)
        {
            //move along path
            MoveAlongPath();
        }
        if(_isRetracting && _retractPath.path.length > _retractDistance)
        {
            Retracting();
        }
        else if(_baseOfVine.parent.gameObject.activeInHierarchy && _isRetracting)
        {
            _isRetracting = false;
            _baseOfVine.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Moves the vine along the given path
    /// </summary>
    private void MoveAlongPath()
    {
        _distance += Time.deltaTime * _speed;
        _followTransform.position = _pathCreator.path.GetPointAtDistance(_distance);
    }

    /// <summary>
    /// Creates the initial instance of the movement audio
    /// </summary>
    private void CreateMovementAudio()
    {
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
        _isAttacking = true;
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
        _followTransform.DOJump(_pathCreator.path.GetPointAtDistance(_distance), .2f, 1, _moveBackAfterAttackTime, false).SetEase(Ease.InOutCubic);
        yield return new WaitForSeconds(_moveBackAfterAttackTime);

        // //attack done
        _isAttacking = false;
        StartMovementAudio();
    }

    /// <summary>
    /// the original way we detected if the player was entering the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        if(IsColliderPlayer(collider) && !_isAttacking)
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
        _isRetracting = true;
        _chainIKRig.weight = 0;
        _dampedTransformRig.weight = 1;
    }
    /// <summary>
    /// retracts the vine
    /// </summary>
    private void Retracting()
    {
        _retractDistance += Time.deltaTime * _retractSpeed;
        _baseOfVine.position = _retractPath.path.GetPointAtDistance(_retractDistance);
        _baseOfVine.right = _retractPath.path.GetDirectionAtDistance(_retractDistance);
    }
    #endregion
}
