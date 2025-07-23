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
using Unity.VisualScripting;

/// <summary>
/// Controls all functionality for the attack procedural animation and room movement
/// </summary>
public class ProceduralVine : MonoBehaviour
{
    [Header("Path Stuff")]
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private float _speed = 5f; // speed of idle path following movement
    private float _idleMoveDistance = 0; //current distance along the path
    [SerializeField] private bool _isWhackAMoleVine = false;

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

    [Header("WAttack Stuff")]
    [SerializeField] private PathCreator _whackAMoleAttackPath; // The target to move toward
    [SerializeField] private float _whackAMoleAttackSpeed = 5f; // Speed of movement
    [SerializeField] private float _whackAMoleAttackDistance = 0;
    private const float _MAX_ATTACK_ANGLE = 70f;
    private const float _ATTACK_DIRECTION_CHECK = 180f;

    [SerializeField] private float _moveBackToPathDuration = .3f;
    [SerializeField] private Animator _animator;

    private Coroutine _whackAMoleSnapAttack;

    private const string APPEAR_ANIMATION_TRIGGER = "appear";
    private const string DISAPPEAR_ANIMATION_TRIGGER = "disappear";
    private const string BITE_ANIMATION_TRIGGER = "bite";

    //state stuff
    public enum EVineState
    {
        attacking,
        retracting,
        appearing,
        shifting,
        whackAMoleAttacking,
        none
    }

    private EVineState _currentState;

    private EventInstance _movementEventInstance;

    public bool IsWhackAMoleVine { get => _isWhackAMoleVine; set => _isWhackAMoleVine = value; }

    /// <summary>
    /// basic initializing upon start
    /// </summary>
    private void Start()
    {
        _currentState = EVineState.none;
        if(_animator.IsUnityNull())
        {
            _animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Updates the vine movement only right now
    /// </summary>
    private void Update() 
    {
        //normal idle vine
        if(!IsWhackAMoleVine)
        {
            //retracting
            if (_currentState == EVineState.retracting && _retractPath.path.length > _retractDistance + .1f)
            {
                Retracting();
                return;
            }

            if (_currentState == EVineState.none)
            {
                //move along path
                MoveAlongPath();
                if (_currentAttackCD > 0)
                {
                    _currentAttackCD -= Time.deltaTime;
                }
            }

            //appearing
            if (_currentState == EVineState.appearing)
            {
                if(_appearPath.path.length > _appearDistance + .1f)
                {
                    Appearing();
                }
                else
                {
                    StartCoroutine(JumpBackToPath(_moveBackToPathDuration));
                }
            }
        }
        //whack a mole vine
        else
        {
            /*//appearing
            if (_currentState == EVineState.appearing && _appearPath.path.length > _appearDistance + .1f)
            {
                Appearing();
            }
            else if(!_isAppeared && _appearPath.path.length <= _appearDistance + .1f) _isAppeared = true;

            //retracting
            if (_currentState == EVineState.retracting)
            {
                if(_retractPath.path.length >= _retractDistance + .1f)
                {
                    Retracting();
                    return;
                }
                else if(_retractPath.path.length < _retractDistance)
                {
                    //.5f delay is a magic number but an important one that shouldn't be changed anywhere
                    Destroy(transform.parent.gameObject, .5f);
                }  
            }

            //wAttacking
            if(_currentState == EVineState.whackAMoleAttacking)
            {
                if(_whackAMoleAttackPath.path.length > _whackAMoleAttackDistance + .1f)
                {
                    WhackAMoleAttack();
                }
                else if(_whackAMoleAttackPath.path.length <= _whackAMoleAttackDistance + .1f && _whackAMoleSnapAttack.IsUnityNull())
                {
                    _whackAMoleSnapAttack = StartCoroutine(WSnapAttack());
                }    
            }*/
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
        if(_currentState != EVineState.retracting && !_isWhackAMoleVine)
        {
            _currentState = EVineState.none;
        }
        
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
        RuntimeSfxManager.Instance.FadeInLoopingOneShot(_movementEventInstance, 
            FmodSfxEvents.Instance.LimbMoveFadeInTime);
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
        RuntimeSfxManager.Instance.FadeOutLoopingOneShot(_movementEventInstance, 
            FmodSfxEvents.Instance.LimbMoveFadeOutTime);
    }

    /// <summary>
    /// Plays audio on spawn
    /// </summary>
    private void PlaySpawnAudio()
    {
        RuntimeSfxManager.APlayOneShotSfxAttached(FmodSfxEvents.Instance.LimbSpawn,_flowerHeadTransform.gameObject);
    }

    /// <summary>
    /// does the movement for the wind up and lunge attack
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    private IEnumerator Attack(Vector3 playerPosition)
    {
        if (_currentState != EVineState.retracting)
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

            if(!IsWhackAMoleVine)
            {
                //move back to og position
                _followTransform.DOJump(_pathCreator.path.GetPointAtDistance(_idleMoveDistance), .2f, 1, _moveBackAfterAttackTime, false).SetEase(Ease.InOutCubic);
                yield return new WaitForSeconds(_moveBackAfterAttackTime);
                // //attack done
                _currentState = EVineState.none;
            }
            else
            {
                StartRetract();
            }
            
            StartMovementAudio();
        }
        
    }

    /// <summary>
    /// starts the whack a mole attack. This specific function sets up the model after appearing.
    /// </summary>
    /// <param name="playerPos"></param>
    public void StartAttack(Vector3 playerPos)
    {
        //magic number .5f for delay otherwise system bugs. Important number that shouldn't be chnaged
        Invoke(nameof(StartWhackAMoleAttack), .5f);
    }

    private Transform _playerTransform;

    /// <summary>
    /// does the actual "strike" to the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator WSnapAttack()
    {
        yield return new WaitForSeconds(_waitAfterRearBackTime);

        //setup and trigger attack
        _animator.SetTrigger(BITE_ANIMATION_TRIGGER);
        _dampedTransformRig.weight = 0f;
        _chainIKRig.weight = .75f;
        _followTransform.position = _flowerHeadTransform.position;
        _currentState = EVineState.whackAMoleAttacking;

        var originalPosition = _flowerHeadTransform.position;

        //redo direction from new position
        var direction = (_playerTransform.position - _followTransform.position).normalized;
        var targetAngle = Vector3.Angle(_followTransform.forward, direction);

        // We can use the dot product to figure out which direction the head is initially facing
        // 0 = wall, -1 = ceiling, 1 = floor
        var initialDirection = Mathf.Round(Vector3.Dot(_followTransform.forward, Vector3.up));
        var savedForward = _followTransform.forward;

        _followTransform.forward = direction;

        // So there's a weird kink with the code that I'm blaming Unity for.
        // For some reason, the following logic is completely inverted when the hallway that the monsters spawn
        // are facing along opposite axes (the x-axis and z-axis).
        // To get around this, we can get the local rotation of the main WhackAMoleAttack prefab and decide
        // if it is x-facing or z-facing and add another check
        bool isFacingZ = Mathf.Round(transform.parent.parent.parent.parent.parent.localEulerAngles.y) % 180 == 0;

        // Attack angle clamping
        // Basically, we "correct" the follow transform's forward vector by shifting it the difference of
        // where it is about to go and where we want it to go.
        if (targetAngle > _MAX_ATTACK_ANGLE)
        {
            if (isFacingZ)
            {
                // Check to see if the vine is coming from a wall or from a ceiling/floor
                if (initialDirection == 0) // wall
                {
                    // "Correct" about the y-axis depending on which side of the vine the player is on
                    if (_followTransform.localEulerAngles.y < _ATTACK_DIRECTION_CHECK)
                    {
                        // There's another weird kink with this segment of code in particular
                        // Whenever the vine initially faces the positive x world direction, it takes the incorrect route
                        // in the logic here.
                        if (Vector3.Dot(savedForward, Vector3.right) > 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, _MAX_ATTACK_ANGLE - targetAngle);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, targetAngle - _MAX_ATTACK_ANGLE);
                        }
                    }
                    else
                    {
                        if (Vector3.Dot(savedForward, Vector3.right) < 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, _MAX_ATTACK_ANGLE - targetAngle);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, targetAngle - _MAX_ATTACK_ANGLE);
                        }
                    }
                }
                else //ceiling/floor
                {
                    // This is a bit more complicated. We need to correct with respect to the xz-plane (the ground/ceiling)
                    // We need to find the culprit. Is it the x-axis, the z-axis, or both?
                    // Let's break up the current forward into its components
                    var forwardX = new Vector3(_followTransform.forward.x, 0, 0);
                    var forwardZ = new Vector3(0, 0, _followTransform.forward.z);

                    var angleX = Vector3.Angle(savedForward, forwardX);
                    var angleZ = Vector3.Angle(savedForward, forwardZ);

                    // We go down the rabbit hole again
                    if (angleX > _MAX_ATTACK_ANGLE)
                    {
                        // We'll check for specifically ceiling or floor this time
                        // Correct with respect to x-axis
                        if (initialDirection < 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.right, _MAX_ATTACK_ANGLE - angleX);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.right, angleX - _MAX_ATTACK_ANGLE);
                        }
                    }
                    if (angleZ > _MAX_ATTACK_ANGLE)
                    {
                        // Correct with respect to z-axis
                        if (initialDirection < 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.forward, _MAX_ATTACK_ANGLE - angleZ);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.forward, angleZ - _MAX_ATTACK_ANGLE);
                        }
                    }
                }
            }
            else
            {
                // This is for x-facing monster segments
                // We are going to do the exact same logic as above, but we are going to invert all of the ending logic
                // Check to see if the vine is coming from a wall or from a ceiling/floor
                if (initialDirection == 0)
                {
                    if (_followTransform.localEulerAngles.y < _ATTACK_DIRECTION_CHECK)
                    {
                        if (Vector3.Dot(savedForward, Vector3.right) < 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, _MAX_ATTACK_ANGLE - targetAngle);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, targetAngle - _MAX_ATTACK_ANGLE);
                        }
                    }
                    else
                    {
                        if (Vector3.Dot(savedForward, Vector3.right) > 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, _MAX_ATTACK_ANGLE - targetAngle);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.up, targetAngle - _MAX_ATTACK_ANGLE);
                        }
                    }
                }
                else
                {
                    var forwardX = new Vector3(_followTransform.forward.x, 0, 0);
                    var forwardZ = new Vector3(0, 0, _followTransform.forward.z);

                    var angleX = Vector3.Angle(savedForward, forwardX);
                    var angleZ = Vector3.Angle(savedForward, forwardZ);

                    if (angleX > _MAX_ATTACK_ANGLE)
                    {
                        if (initialDirection > 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.right, _MAX_ATTACK_ANGLE - angleX);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.right, angleX - _MAX_ATTACK_ANGLE);
                        }
                    }
                    if (angleZ > _MAX_ATTACK_ANGLE)
                    {
                        if (initialDirection > 0)
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.forward, _MAX_ATTACK_ANGLE - angleZ);
                        }
                        else
                        {
                            _followTransform.RotateAround(_followTransform.position, Vector3.forward, angleZ - _MAX_ATTACK_ANGLE);
                        }
                    }
                }
            }
        }

        // Get the position to strike based on the follow transform and distance to player
        var strikePos = _followTransform.position + _followTransform.forward * 
            Vector3.Distance(_followTransform.position, _playerTransform.position) + Vector3.up * .3f;

        //snaps to player
        _followTransform.DOMove(strikePos, _lungeToPlayerDuration * .75f, false).SetEase(Ease.OutCubic);
        //Plays attack audio
        RuntimeSfxManager.APlayOneShotSfxAttached(FmodSfxEvents.Instance.LimbAttack, _flowerHeadTransform.gameObject);
        yield return new WaitForSeconds(_lungeToPlayerDuration);

        StartCoroutine(LerpChainIKWeight(_chainIK.weight, .25f, .25f));

        // Move the head back to the original position to make it look cleaner
        _followTransform.DOMove(originalPosition, _lungeToPlayerDuration * .75f, false).SetEase(Ease.OutCubic);

        //trigger retract and then destroy
        DisappearWhackAMole();
    }

    /// <summary>
    /// the function for destorying the whackamole system. Called when a vine is shot
    /// </summary>
    public void DisappearWhackAMole()
    {
        StopCoroutine(_whackAMoleSnapAttack);
        _animator.SetTrigger(DISAPPEAR_ANIMATION_TRIGGER);
        Destroy(transform.parent.gameObject, 1.167f);
    }

    /// <summary>
    /// lerps ChainIK rig weight
    /// </summary>
    /// <param name="weight1"></param>
    /// <param name="weight2"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator LerpChainIKWeight(float weight1, float weight2, float time)
    {
        float elapsed = 0f;
        float percentToCompletion;
        while (elapsed < time)
        {
            percentToCompletion = elapsed / time;
            _chainIK.weight = Mathf.Lerp(weight1, weight2, percentToCompletion);


            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// the original way we detected if the player was entering the room
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerStay(Collider collider)
    {
        if(IsColliderPlayer(collider) && _currentState != EVineState.attacking && _currentState != EVineState.retracting && _currentAttackCD <= 0 && _isAppeared)
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
    /// <param name="player"></param>
    public void StartAppear(Transform player)
    {
        //skip if already appearing or appeared
        if(_currentState == EVineState.appearing || _isAppeared)
        {
            return;
        }

        _playerTransform = player;

        //shift the rig to used the dampedTransform instead of IK
        _appearDistance = 0;
        _currentState = EVineState.appearing;

        //trigger animation
        _animator.SetTrigger(APPEAR_ANIMATION_TRIGGER);

        //trigger attack to happen after certain point in animation
        //change rig to use chainIK
        _whackAMoleSnapAttack =  StartCoroutine(WSnapAttack());

        //play sfx
        CreateMovementAudio();
        StartMovementAudio();
        PlaySpawnAudio();
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

    #region WhackAmoleAttack

    /// <summary>
    /// sets up the vine for the attack
    /// </summary>
    public void StartWhackAMoleAttack()
    {
        //shift the rig to used the dampedTransform instead of IK
        _whackAMoleAttackDistance = 0;
        
        //change rig to use chainIK
        _dampedTransformRig.weight = 0f;
        _chainIKRig.weight = 1f;
        _followTransform.position = _whackAMoleAttackPath.path.GetPointAtDistance(0);
        _rigBuilder.Build();
        _currentState = EVineState.whackAMoleAttacking;
        StartCoroutine(WSnapAttack());
    }

    /// <summary>
    ///  this rears the whack a mole attack vine back before attacking
    /// </summary>
    private void WhackAMoleAttack()
    {
        //makes the base of the vine start moving up, cannot make system follow the head(it doesn't work)
        _whackAMoleAttackDistance += Time.deltaTime * _whackAMoleAttackSpeed;
        _followTransform.position = _whackAMoleAttackPath.path.GetPointAtDistance(_whackAMoleAttackDistance);
    }
    #endregion

    public bool GetIsAppeared() => _isAppeared;
    public EVineState GetVineState() => _currentState;
}
