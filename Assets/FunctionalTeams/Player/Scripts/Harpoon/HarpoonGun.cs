/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarpoonGun : MonoBehaviour
{
    #region Inspector Variables
    [Header("Harpoon Variables")]
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon
    [Tooltip("The speed the harpoon moves in the launch direction")]
    [SerializeField] private float _harpoonSpeed = 50f; // Speed of the harpoon
    [Tooltip("max distance the harpoon can launch")]
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [Tooltip("the max duration the harpoon can take to reel in(at max distance)")]
    [SerializeField] private float _reelDuration = 3f; // Time to reel in at max distance
    [Tooltip("cooldown of the gun after fully reeled in")]
    [SerializeField] private float _gunCooldown = 2f; // cd of harpoon gun after fully retracted
    [Tooltip("if true then you have to hold mouse down to retract fully. if false retracts automatically")]
    [SerializeField] private bool _holdToRetractMode = true; // turns on or off having to hold mouse down to retract
    [Tooltip("Specifies if the harpoon can deal damage while being reeled back")]
    [SerializeField] private bool _dealsDamageWhenReturning = false;

    [Space]
    [Tooltip("The time it takes to reach max focus")]
    [SerializeField] private float _focusTime;
    [Tooltip("The accuracy that you start at when you begin focusing")]
    [Range(0, .5f)]
    [SerializeField] private float _focusStartingInaccuracy;
    [Tooltip("The curve at which the accuracy increases while focusing")]
    [SerializeField] private AnimationCurve _focusCurve;

    [Header("Harpoon Functionality Dependencies")]
    [Tooltip("Transform of whatever the cameras rotation is. Probably the cinemachine camera object")]
    [SerializeField] private Transform _playerLookDirection;
    [Tooltip("name of shoot animation")]
    [SerializeField] private string _harpoonShootTrigger = "shoot";
    [Tooltip("name of retract animation")]
    [SerializeField] private string _harpoonRetractTrigger = "drawBack";
    [Tooltip("Transform on the end of the harpoon gun (whereever the harpoon comes out of)")]
    [SerializeField] private Transform _harpoonTip;
    [Tooltip("The harpoon object on the gun. Disappears and reappears to indicate whether player has shot ready")]
    [SerializeField] private GameObject _harpoonOnGun;
    [Tooltip("Layers the launched harpoon can not hit")]
    [SerializeField] private LayerMask _excludeLayers;
    [Tooltip("The input action for shooting")]
    [SerializeField] private InputActionReference _harpoonShoot;
    [Tooltip("The input action for focusing")]
    [SerializeField] private InputActionReference _harpoonFocus;
    [Tooltip("The input action for retracting")]
    [SerializeField] private InputActionReference _harpoonRetract;

    [Header("Camera Shake Values")]
    [Tooltip("Recoil Intensity Shake")]
    [SerializeField] private float _recoilCameraShakeIntensity = 5f;
    [Tooltip("Recoil time Shake")]
    [SerializeField] private float _recoilCameraShakeTime = 0.05f;
    [Tooltip("Retract Intensity Shake")]
    [SerializeField] private float _retractCameraShakeIntensity = 3f;
    [Tooltip("Retract time Shake")]
    [SerializeField] private float _retractCameraShakeTime = .05f;
    #endregion

    //private variables
    private GameObject _harpoonSpear;
    private Animator _harpoonAnimator;
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    private bool _isShooting;
    private bool _isFocusing = false;
    private Coroutine _focusingCoroutine;
    private float _currentFocus = 0;
    private RaycastHit _hit;
    private float _currentReelDur;
    private HarpoonRope _harpoonRope;

    private PlayerInputMap _playerInputMap;

    private void Awake(){
        _harpoonRope = GetComponent<HarpoonRope>();
        _harpoonAnimator = GetComponent<Animator>();
    }

    /// sets up the button for shooting
    /// </summary>
    public void SubscribeInput()
    {
        _harpoonShoot.action.performed += FireHarpoon;

        _harpoonFocus.action.started += FocusHarpoon;
        _harpoonFocus.action.canceled += StopHarpoonFocusing;
    }

    /// <summary>
    /// disables shoot button
    /// </summary>
    public void UnsubscribeInput()
    {
        _harpoonShoot.action.performed -= FireHarpoon;

        _harpoonFocus.action.started -= FocusHarpoon;
        _harpoonFocus.action.canceled -= StopHarpoonFocusing;
    }

    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    private void FireHarpoon(InputAction.CallbackContext context)
    {
        //dont shoot if already shot
        if(_harpoonSpear != null || _isReeling || !_isFocusing)
        {
            return;
        }
        // Instantiate the harpoon and set its initial position and direction
        _harpoonSpear = Instantiate(_harpoonPrefab, _playerLookDirection.position, Quaternion.identity);
        _harpoonSpear.SetActive(false);
        _harpoonSpear.transform.GetChild(0).rotation = _playerLookDirection.rotation;
        _isShooting = true;
        _fireDir = GetHarpoonDirectionWithFocus(); // In the direction the player is looking
        // Start moving the harpoon
        StartCoroutine(MoveHarpoon());
        _harpoonOnGun.SetActive(false);

        //Camera shake here when combined with Stapay

        PlayerManager.Instance.InvokeHarpoonFiredEvent();
    }

    /// <summary>
    /// Adds spread to the weapon relative to focus
    /// </summary>
    /// <returns></returns>
    private Vector3 GetHarpoonDirectionWithFocus()
    {
        //Multiplies the direction the player is looking by a random variance scaled by current focus
        return _playerLookDirection.forward + (UnityEngine.Random.insideUnitSphere * _currentFocus) ;
    }

    #region Focusing
    /// <summary>
    /// Starts focusing the weapon
    /// </summary>
    /// <param name="context"></param>
    private void FocusHarpoon(InputAction.CallbackContext context)
    {
        _isFocusing = true;
        _focusingCoroutine = StartCoroutine(FocusProcess());

        PlayerManager.Instance.InvokeHarpoonFocusStartEvent();
    }

    /// <summary>
    /// The process of focusing the weapon over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator FocusProcess()
    {
        float focusCompletion = 0;
        while(focusCompletion < 1)
        {
            //Increases the progress on focusing
            focusCompletion += Time.deltaTime / _focusTime;

            //Sets the current focus based on the animation graph and inaccuracy scalar
            _currentFocus = _focusCurve.Evaluate(focusCompletion) * _focusStartingInaccuracy;

            yield return null;
        }
        _currentFocus = 0;
    }

    /// <summary>
    /// Stops the focusing of the weapon
    /// </summary>
    /// <param name="context"></param>
    private void StopHarpoonFocusing(InputAction.CallbackContext context)
    {
        StopCoroutine(_focusingCoroutine);
        _currentFocus = 0;
        _isFocusing = false;

        PlayerManager.Instance.InvokeHarpoonEndEvent();
    }

    #endregion

    /// <summary>
    /// Enables the harpoon
    /// </summary>
    private void SetHarpoonActive()
    {
        //delays visual of harpoon appearing for better appearance
        _harpoonSpear.SetActive(true);
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    private IEnumerator MoveHarpoon()
    {
        Invoke(nameof(SetHarpoonActive), .15f);
        _currentDist = 0f;
        while (_currentDist < _maxDistance && !_isReeling)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = _fireDir * _harpoonSpeed * Time.deltaTime;
            // Cast a ray from the harpoon's current position forward by the amount it moves this frame
            if (Physics.Raycast(_harpoonSpear.transform.position, movement, out _hit, 1f, ~_excludeLayers))
            {
                //if hit grabbable object then child object to harpoon to bring object back with harpoon
                if(_hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
                {
                    _hit.transform.SetParent(_harpoonSpear.transform);
                    Rigidbody hitRB = _hit.transform.GetComponent<Rigidbody>();
                    if(hitRB != null)
                    {
                        hitRB.isKinematic = true;
                    }
                }
                // Harpoon _hit something, stop its movement and start reeling it in
                _harpoonSpear.transform.position = _hit.point; // Snap the harpoon to the _hit point
                StartReeling(_hit.point);
                yield break; // Exit the coroutine, no need to keep moving the harpoon
            }

            // If no collision, move the harpoon
            _harpoonSpear.transform.Translate(movement);
            _currentDist += movement.magnitude;

            yield return null;
        }
        // If max distance reached and no _hit, start reeling back
        if (!_isReeling)
        {
            StartReeling(_hit.point);
        }
    }

    /// <summary>
    /// sets up the reel positions and time. if the target hit is grabbable child it to the harpoon so it comes back
    /// </summary>
    /// <param name="_hitPosition"></param>
    private void StartReeling(Vector3 _hitPosition)
    {
        _isReeling = true;

        _harpoonSpear.GetComponentInChildren<Collider>().enabled = _dealsDamageWhenReturning;
        
        float distanceFromPlayer;
        if(_hit.transform != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, _hitPosition);
        }else
        {
            distanceFromPlayer = _maxDistance;
        }
        // Adjust the reeling time based on the distance
        _currentReelDur = distanceFromPlayer / _maxDistance * _reelDuration;
        StartCoroutine(ReelHarpoon());
    }

    /// <summary>
    /// actually reels in the harpoon and destorys the instaniated harpoon. Begins reload timer at the end.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReelHarpoon()
    {
        yield return new WaitForSeconds(.05f);
        float elapsedTime = 0;
        if(!_holdToRetractMode)
        {
            //cause the wave action again when reeling

            //Not entirely sure why its set up to do both events, but I'm simply
            //replicating the current event structure in the player manager
            PlayerManager.Instance.InvokeHarpoonRetractEvent();
            PlayerManager.Instance.InvokeHarpoonFiredEvent();
        }
        // Lerp the harpoon back to the player over time
        var startPos = _harpoonSpear.transform.position;
        bool startedRetracting = false;
        while(elapsedTime < _currentReelDur)
        {
            //if hold to retract is on, only pull in harpoon if holding down button
            if(_holdToRetractMode)
            {
                if(_harpoonRetract.action.inProgress)
                {
                    if(!startedRetracting)
                    {
                        startedRetracting = true;
                        //cause the wave action again when reeling
                        //Not entirely sure why its set up to do both events, but I'm simply
                        //replicating the current event structure in the player manager
                        PlayerManager.Instance.InvokeHarpoonRetractEvent();
                        PlayerManager.Instance.InvokeHarpoonFiredEvent();
                    }
                    _isShooting = true;
                    _harpoonSpear.transform.GetChild(0).LookAt(_harpoonTip);
                    _harpoonSpear.transform.position = Vector3.Lerp(startPos, _harpoonTip.position, elapsedTime/ _currentReelDur);
                    elapsedTime+= Time.deltaTime;
                } 
                //otherwise automatically pull in 
            }
            else
            {
                _harpoonSpear.transform.GetChild(0).LookAt(_harpoonTip);
                _harpoonSpear.transform.position = Vector3.Lerp(startPos, _harpoonTip.position, elapsedTime/ _currentReelDur);
                elapsedTime+= Time.deltaTime; 
            }
            yield return null;
        }
        // get rid of harpoon
        if(_hit.transform != null)
        {
            _hit.transform.SetParent(null);
            if(_hit.transform.GetComponent<Rigidbody>() != null)
            {
                _hit.transform.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        //Camera shake here when combined with Stapay
        StartCoroutine(ResetHarpoon());

        PlayerManager.Instance.InvokeHarpoonRetractEvent();
    }

    /// <summary>
    /// Resets the harpoon and "reloads" it
    /// </summary>
    private IEnumerator ResetHarpoon()
    {
        _hit = new RaycastHit();
        Destroy(_harpoonSpear);
        _isShooting = false;
        yield return new WaitForSeconds(_gunCooldown);
        _isReeling = false;
        _harpoonOnGun.SetActive(true);
    }


    #region Getters
    //exposed variables
    public GameObject GetHarpoonSpear() => _harpoonSpear;
    public bool GetIsShooting() => _isShooting;
    public Transform GetHarpoonTip() => _harpoonTip;
    #endregion
}
