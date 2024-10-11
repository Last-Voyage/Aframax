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

/// <summary>
/// Provides the functionality for the harpoon weapon
/// </summary>
public class HarpoonGun : MonoBehaviour
{
    #region Variables
    [Header("Harpoon Variables")]
    [Tooltip("The speed the harpoon moves in the launch direction")]
    [SerializeField] private float _harpoonSpeed = 50f; // Speed of the harpoon
    [Tooltip("The speed the harpoon projectile moves back towards the player")]
    [SerializeField] private float _reelSpeed;
    [Tooltip("Max distance the harpoon can launch")]
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [Tooltip("Cooldown of the gun after fully reeled in")]
    [SerializeField] private float _gunCooldown = 2f; // cd of harpoon gun after fully retracted
    [Tooltip("If true then you have to hold mouse down to retract fully. if false retracts automatically")]
    [SerializeField] private bool _holdToRetractMode = true; // turns on or off having to hold mouse down to retract
    [Tooltip("The projectile being fired")]
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon
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

    private GameObject _harpoonSpear;
    
    private bool _isReeling = false;
    private Vector3 _fireDir;
    private float _currentDist;
    private bool _isShooting;

    private RaycastHit _hit;

    private PlayerInputMap _playerInputMap;
    #endregion

    #region Setup
    private void Awake()
    {
        _harpoonAnimator = GetComponent<Animator>();

        CreateInitialHarpoonProjectile();

        StartCoroutine(HarpoonCameraOrientation());
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
    }

    /// <summary>
    /// disables shoot button
    /// </summary>
    public void UnsubscribeInput()
    {
        _harpoonShoot.action.performed -= FireHarpoon;

        _harpoonFocus.action.started -= FocusHarpoon;
        _harpoonFocus.action.canceled -= StartUnfocusingHarpoon;
    }
    #endregion

    #region Harpoon Firing
    /// <summary>
    /// creates a harpoon, sets up the fire direction and everything else to begin the launch
    /// </summary>
    private void FireHarpoon(InputAction.CallbackContext context)
    {
        //dont shoot if already shot
        if (_isShooting || _isReeling || _currentFocusState != EFocusState.Focusing)
        {
            return;
        }
        
        //Sets the spears initial location
        _harpoonSpear.transform.position = _playerLookDirection.position;

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
    /// Enables the harpoon
    /// </summary>
    private void SetHarpoonActive()
    {
        //delays visual of harpoon appearing for better appearance
        _harpoonSpear.SetActive(true);
        Debug.Log("Setactive");
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
                if (_hit.transform.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
                {
                    _hit.transform.SetParent(_harpoonSpear.transform);
                    Rigidbody hitRB = _hit.transform.GetComponent<Rigidbody>();
                    if (hitRB != null)
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
    #endregion

    #region Harpoon Reeling
    /// <summary>
    /// sets up the reel positions and time. if the target hit is grabbable child it to the harpoon so it comes back
    /// </summary>
    /// <param name="_hitPosition"></param>
    private void StartReeling(Vector3 _hitPosition)
    {
        _isReeling = true;
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
        if (!_holdToRetractMode)
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
        while (Vector3.Distance(transform.position, _harpoonSpear.transform.position) > .1f)
        {
            //if hold to retract is on, only pull in harpoon if holding down button
            if (_holdToRetractMode)
            {
                if (_harpoonRetract.action.inProgress)
                {
                    if (!startedRetracting)
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
                    HarpoonReelProjectileMovement();
                    elapsedTime += Time.deltaTime;
                }
                //otherwise automatically pull in 
            }
            else
            {
                _harpoonSpear.transform.GetChild(0).LookAt(_harpoonTip);
                HarpoonReelProjectileMovement();
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
        // get rid of harpoon
        if (_hit.transform != null)
        {
            _hit.transform.SetParent(null);
            if (_hit.transform.GetComponent<Rigidbody>() != null)
            {
                _hit.transform.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        //Camera shake here when combined with Stapay
        StartCoroutine(ResetHarpoon());

        PlayerManager.Instance.InvokeHarpoonRetractEvent();
    }

    /// <summary>
    /// Moves the harpoon projectile back to the player
    /// </summary>
    private void HarpoonReelProjectileMovement()
    {
        Vector3 direction = (transform.position - _harpoonSpear.transform.position).normalized;
        _harpoonSpear.transform.position += direction * Time.deltaTime * _reelSpeed;
    }
    #endregion

    #region Reloading
    /// <summary>
    /// Resets the harpoon and "reloads" it
    /// </summary>
    private IEnumerator ResetHarpoon()
    {
        _hit = new RaycastHit();
        _harpoonSpear.SetActive(false);
        _isShooting = false;
        yield return new WaitForSeconds(_gunCooldown);
        _isReeling = false;
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
        _currentFocusAccuracy = 0;
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
        //Sets the current focus based on the animation graph and inaccuracy scalar
        _currentFocusAccuracy = _focusCurve.Evaluate(_focusProgress) * _focusStartingInaccuracy;
    }

    /// <summary>
    /// Adds spread to the weapon relative to focus
    /// </summary>
    /// <returns></returns>
    private Vector3 GetHarpoonDirectionWithFocus()
    {
        //Multiplies the direction the player is looking by a random variance scaled by current focus
        return _playerLookDirection.forward + (UnityEngine.Random.insideUnitSphere * _currentFocusAccuracy);
    }
    #endregion

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

    #region Getters
    //Getters for private variables
    public GameObject GetHarpoonSpear() => _harpoonSpear;
    public bool GetIsShooting() => _isShooting;
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
