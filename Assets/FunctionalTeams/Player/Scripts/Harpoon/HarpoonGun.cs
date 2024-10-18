/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
//                     Ryan Swanson
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides the functionality for the harpoon weapon
/// </summary>
public class HarpoonGun : MonoBehaviour
{
    public static HarpoonGun Instance;

    #region Variables
    [Header("Harpoon Variables")]
    [Tooltip("The speed the harpoon moves in the launch direction")]
    [SerializeField] private float _fireSpeed = 50f; // Speed of the harpoon
    [Tooltip("The speed the harpoon projectile moves back towards the player")]
    [SerializeField] private float _reelSpeed;
    [Tooltip("The time between when the harpoon hits its target or distance limit to when you can start reeling")]
    [SerializeField] private float _reelStartDelay;
    [Tooltip("Max distance the harpoon can launch")]
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [Tooltip("Cooldown of the gun after fully reeled in")]
    [SerializeField] private float _reloadTime = 2f; // cd of harpoon gun after fully retracted
    [Tooltip("If true then you have to hold mouse down to retract fully. if false retracts automatically")]
    [SerializeField] private bool _holdToRetractMode = true; // turns on or off having to hold mouse down to retract
    [Tooltip("The projectile being fired")]
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon

    private bool _reelingButtonHeld;

    private GameObject _harpoonSpear;
    private Collider _harpoonCollider;

    private EHarpoonFiringState _harpoonFiringState;

    [Space]
    [Header("Focus")]
    [Tooltip("The time it takes to reach max focus")]
    [SerializeField] private float _focusTime;
    [Tooltip("The time to unfocus the weapon")]
    [SerializeField] private float _unfocusTime;
    [Tooltip("The accuracy that you start at when you begin focusing")]
    [Range(0, .5f)]
    [SerializeField] private float _focusStartingInaccuracy;
    [Tooltip("The curve at which the accuracy increases while focusing")]
    [SerializeField] private AnimationCurve _focusCurve;

    private float _currentFocusAccuracy = 0;

    private float _focusProgress = 0;
    private EFocusState _currentFocusState;

    private Coroutine _focusUnfocusCoroutine;

    [Space]
    [Header("Harpoon Functionality Dependencies")]
    [Tooltip("Transform of whatever the cameras rotation is. Probably the cinemachine camera object")]
    [SerializeField] private Transform _playerLookDirection;
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

    [Space]
    [Header("Camera Shake Values")]
    [Tooltip("Recoil Intensity Shake")]
    [SerializeField] private float _recoilCameraShakeIntensity = 5f;
    [Tooltip("Recoil time Shake")]
    [SerializeField] private float _recoilCameraShakeTime = 0.05f;
    [Tooltip("Retract Intensity Shake")]
    [SerializeField] private float _retractCameraShakeIntensity = 3f;
    [Tooltip("Retract time Shake")]
    [SerializeField] private float _retractCameraShakeTime = .05f;

    [Space]
    [Header("Animation")]
    [Tooltip("name of shoot animation")]
    [SerializeField] private string _harpoonShootTrigger = "shoot";
    [Tooltip("name of retract animation")]
    [SerializeField] private string _harpoonRetractTrigger = "drawBack";
    private Animator _harpoonAnimator;
    
    private Vector3 _fireDir;
    private float travelDistance;

    private PlayerInputMap _playerInputMap;
    #endregion

    #region Setup
    private void Awake()
    {
        EstablishInstance();

        _harpoonAnimator = GetComponent<Animator>();

        CreateInitialHarpoonProjectile();

        StartCoroutine(HarpoonCameraOrientation());
    }

    /// <summary>
    /// Establishes the instance and removes
    /// </summary>
    private void EstablishInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Creates the harpoon projectile
    /// Rather than recreating it everytime you fire
    /// I am literally object pooling a single object
    /// This is more efficient ever so slightly
    /// </summary>
    private void CreateInitialHarpoonProjectile()
    {
        _harpoonSpear = Instantiate(_harpoonPrefab, _playerLookDirection.position, Quaternion.identity);
        _harpoonCollider = _harpoonSpear.GetComponentInChildren<Collider>();
        _harpoonSpear.SetActive(false);
    }
    #endregion

    #region Input
    /// sets up the button for shooting
    /// </summary>
    public void SubscribeInput()
    {
        _harpoonShoot.action.performed += FireHarpoon;

        _harpoonFocus.action.started += FocusHarpoon;
        _harpoonFocus.action.canceled += StartUnfocusingHarpoon;

        _harpoonRetract.action.started += ReelButtonHeld;
        _harpoonRetract.action.canceled += ReelButtonReleased;
    }

    /// <summary>
    /// disables shoot button
    /// </summary>
    public void UnsubscribeInput()
    {
        _harpoonShoot.action.performed -= FireHarpoon;

        _harpoonFocus.action.started -= FocusHarpoon;
        _harpoonFocus.action.canceled -= StartUnfocusingHarpoon;

        _harpoonRetract.action.started -= ReelButtonHeld;
        _harpoonRetract.action.canceled -= ReelButtonReleased;
    }
    #endregion

    #region Harpoon Firing
    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    private void FireHarpoon(InputAction.CallbackContext context)
    {
        //Return when we can't shoot
        if (_harpoonFiringState != EHarpoonFiringState.Ready || _currentFocusState != EFocusState.Focusing)
        {
            return;
        }
        
        //Sets the spears initial location
        _harpoonSpear.transform.position = _harpoonTip.transform.position;

        _harpoonSpear.SetActive(true);

        _harpoonCollider.enabled = true;

        // Get the direction the harpoon is fired
        _fireDir = GetHarpoonDirectionWithFocus(); 

        //Rotates the harpoon projectile to look in the fire direction
        //harpoonSpear.transform.LookAt(_harpoonSpear.transform.position + _fireDir);

        SetHarpoonProjectileLookAt(_harpoonSpear.transform.position + _fireDir);

        _harpoonFiringState = EHarpoonFiringState.Firing;

        // Start moving the harpoon
        StartCoroutine(HarpoonFireProcess());

        // Personally I think the projectile should be the same as the object on the visual as the gun itself, 
        // but that's a discussion for a later day
        _harpoonOnGun.SetActive(false);

        //Camera shake here when combined with Stapay
        CinemachineShake.Instance.ShakeCamera(_recoilCameraShakeIntensity, _recoilCameraShakeTime);

        PlayerManager.Instance.InvokeHarpoonFiredStartEvent();
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    private IEnumerator HarpoonFireProcess()
    {
        travelDistance = 0f;
        while (travelDistance < _maxDistance)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = _fireDir * _fireSpeed * Time.deltaTime;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame
            if (Physics.Raycast(_harpoonSpear.transform.position, 
                movement, out RaycastHit hit, movement.magnitude, ~_excludeLayers))
            {
                // Harpoon _hit something, stop its movement and start reeling it in
                _harpoonSpear.transform.position = hit.point; // Snap the harpoon to the _hit point
                break;
            }

            // If no collision, move the harpoon
            HarpoonFiredProjectileMovement(movement);
            travelDistance += movement.magnitude;

            yield return null;
        }
        //Either reached here because we hit something or because we have exceeded the max distance
        PlayerManager.Instance.InvokeHarpoonFiredEndEvent();

        StartReelingProcess();
    }

    /// <summary>
    /// Moves the harpoon when its being fired out
    /// </summary>
    /// <param name="movement"></param>
    private void HarpoonFiredProjectileMovement(Vector3 movement)
    {
        _harpoonSpear.transform.position += movement;
    }
    #endregion

    #region Harpoon Reeling
    private void ReelButtonHeld(InputAction.CallbackContext context)
    {
        _reelingButtonHeld = true;
        PlayerManager.Instance.InvokeHarpoonRetractStartEvent();
    }

    private void ReelButtonReleased(InputAction.CallbackContext context)
    {
        _reelingButtonHeld = false;
        PlayerManager.Instance.InvokeHarpoonRetractStoppedEvent();
    }

    /// <summary>
    /// sets up the reel positions and time. if the target hit is grabbable child it to the harpoon so it comes back
    /// </summary>
    /// <param name="_hitPosition"></param>
    private void StartReelingProcess()
    {
        _harpoonFiringState = EHarpoonFiringState.Reeling;

        StartCoroutine(ReelHarpoonProcess());
    }

    /// <summary>
    /// actually reels in the harpoon and destorys the instaniated harpoon. Begins reload timer at the end.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReelHarpoonProcess()
    {
        yield return new WaitForSeconds(_reelStartDelay);

        _harpoonCollider.enabled = false;
        //Eventually I imagine the _holdToRetractMode will be removed

        if (!_holdToRetractMode)
        {
            PlayerManager.Instance.InvokeHarpoonRetractStartEvent();
        }

        while (Vector3.Distance(_harpoonTip.transform.position, _harpoonSpear.transform.position) > .1f)
        {
            //if hold to retract is on, only pull in harpoon if holding down button
            if (_holdToRetractMode)
            {
                if (_reelingButtonHeld)
                {
                    SetHarpoonProjectileLookAt(_harpoonTip.transform.position);
                    HarpoonReelProjectileMovement();
                }
                //otherwise automatically pull in 
            }
            else
            {
                SetHarpoonProjectileLookAt(_harpoonTip.transform.position);
                HarpoonReelProjectileMovement();
            }
            yield return null;
        }

        HarpoonFullyReeled();
    }

    /// <summary>
    /// Called when the harpoon projectile has retracted fully back to the player
    /// </summary>
    private void HarpoonFullyReeled()
    {
        PlayerManager.Instance.InvokeHarpoonFullyReeledEvent();
        //Camera shake here when combined with Stapay
        CinemachineShake.Instance.ShakeCamera(_retractCameraShakeIntensity, _retractCameraShakeTime);

        StartCoroutine(ResetHarpoon());
    }

    /// <summary>
    /// Moves the harpoon projectile back to the player
    /// </summary>
    private void HarpoonReelProjectileMovement()
    {
        Vector3 direction = (_harpoonTip.transform.position - _harpoonSpear.transform.position).normalized;
        _harpoonSpear.transform.position += direction * Time.deltaTime * _reelSpeed;
    }
    #endregion

    #region Reloading
    /// <summary>
    /// Resets the harpoon and "reloads" it
    /// </summary>
    private IEnumerator ResetHarpoon()
    {
        _harpoonSpear.SetActive(false);
        _harpoonFiringState = EHarpoonFiringState.Reloading;

        yield return new WaitForSeconds(_reloadTime);

        _harpoonFiringState = EHarpoonFiringState.Ready;
        _harpoonOnGun.SetActive(true);
    }
    #endregion

    #region Focusing
    /// <summary>
    /// Starts focusing the weapon
    /// </summary>
    /// <param name="context"></param>
    private void FocusHarpoon(InputAction.CallbackContext context)
    {
        _currentFocusState = EFocusState.Focusing;

        StopCurrentFocusCoroutine();
        _focusUnfocusCoroutine = StartCoroutine(FocusProcess());

        PlayerManager.Instance.InvokeHarpoonFocusStartEvent();
    }

    /// <summary>
    /// Stops the focusing of the weapon
    /// </summary>
    /// <param name="context"></param>
    private void StartUnfocusingHarpoon(InputAction.CallbackContext context)
    {
        _currentFocusState = EFocusState.Unfocusing;

        StopCurrentFocusCoroutine();
        _focusUnfocusCoroutine = StartCoroutine(UnfocusProcess());

        PlayerManager.Instance.InvokeHarpoonFocusEndEvent();
    }

    /// <summary>
    /// Stops the process of focusing or unfocusing
    /// </summary>
    private void StopCurrentFocusCoroutine()
    {
        if(_focusUnfocusCoroutine != null)
        {
            StopCoroutine(_focusUnfocusCoroutine);
        }
    }

    /// <summary>
    /// The process of focusing the weapon over time
    /// </summary>
    /// <returns></returns>
    private IEnumerator FocusProcess()
    {
        while(_focusProgress < 1)
        {
            //Increases the progress on focusing
            _focusProgress += Time.deltaTime / _focusTime;

            CalculateCurrentFocusAccuracy();

            yield return null;
        }

        FocusMax();
    }

    private void FocusMax()
    {
        PlayerManager.Instance.InvokeHarpoonFocusMaxEvent();

        _focusProgress = 1;
        _currentFocusAccuracy = 0;
    }

    /// <summary>
    /// The process of unfocusing the weapon
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnfocusProcess()
    {
        while(_focusProgress > 0)
        {
            _focusProgress -= Time.deltaTime / _unfocusTime;

            CalculateCurrentFocusAccuracy();

            yield return null;
        }

        WeaponFullyUnfocused();
    }

    /// <summary>
    /// Is called when the weapon is completely unfocused
    /// </summary>
    private void WeaponFullyUnfocused()
    {
        _currentFocusState = EFocusState.None;

        _focusProgress = 0;
    }

    /// <summary>
    /// Determines what the current focus accuracy is based on the focus progress
    /// </summary>
    private void CalculateCurrentFocusAccuracy()
    {
        //This function is currently being done every time the focus progress is updated,
        //  you could argue that it should only be done in GetHarpoonDirectionWithFocus.
        //Currently the only time you need to know the current accuracy is when you shoot, but if that were
        //  to change then it should be how it is currently setup

        //Sets the current focus based on the animation graph and inaccuracy scalar
        _currentFocusAccuracy = _focusCurve.Evaluate(_focusProgress) * _focusStartingInaccuracy;
    }

    /// <summary>
    /// Adds spread to the weapon relative to focus
    /// </summary>
    /// <returns></returns>
    private Vector3 GetHarpoonDirectionWithFocus()
    {
        //Determines the direction from the harpoon tip to the point the mouse is at
        Vector3 endDir = (((_playerLookDirection.forward * _maxDistance) + transform.position) 
            - _harpoonTip.transform.position).normalized;

        //Multiplies the direction the player is looking by a random variance scaled by current focus
        return endDir + (UnityEngine.Random.insideUnitSphere * _currentFocusAccuracy);
    }
    #endregion

    #region General
    /// <summary>
    /// Sets the harpoon projectile to look at something
    /// </summary>
    /// <param name="target"></param>
    private void SetHarpoonProjectileLookAt(Vector3 target)
    {
        _harpoonSpear.transform.LookAt(target);
    }

    /// <summary>
    /// Maintains the orientation of the harpoon relative to camera direction
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarpoonCameraOrientation()
    {
        while(true)
        {
            transform.rotation = _playerLookDirection.rotation;
            yield return null;
        }
    }
    #endregion

    #region Getters
    //Getters for private variables
    public GameObject GetHarpoonSpear() => _harpoonSpear;
    public EHarpoonFiringState GetHarpoonFiringState() => _harpoonFiringState;
    public Transform GetHarpoonTip() => _harpoonTip;
    #endregion
}

/// <summary>
/// Contains the state in which the harpoon shooting functionality is in
/// </summary>
public enum EHarpoonFiringState
{
    Ready,
    Firing,
    Reeling,
    Reloading
};

/// <summary>
/// Contains the state in which the harpoon focusing is currently in
/// </summary>
public enum EFocusState
{
    None,
    Focusing,
    Unfocusing
};
