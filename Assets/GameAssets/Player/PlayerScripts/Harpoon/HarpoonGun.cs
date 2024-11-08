/*****************************************************************************
// File Name :         HarpoonGun.cs
// Author :            Tommy Roberts
//                     Ryan Swanson
//                     Adam Garwacki
// Creation Date :     9/22/2024
//
// Brief Description : Controls the basic shoot harpoon and retract functionality.
*****************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Provides the functionality for the harpoon weapon
/// </summary>
public class HarpoonGun : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// Contains the state in which the harpoon shooting functionality is in
    /// </summary>
    public enum EHarpoonFiringState
    {
        Ready,
        Firing,
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
    #endregion

    #region Variables
    [Header("Harpoon Variables")]
    [Tooltip("The speed the harpoon moves in the launch direction")]
    [SerializeField] private float _fireSpeed = 50f; // Speed of the harpoon
    [Tooltip("Max distance the harpoon can launch")]
    [SerializeField] private float _maxDistance = 100f; // Max travel distance
    [Tooltip("Cooldown of the gun after fully reeled in")]
    [SerializeField] private float _reloadTime = 2f; // cd of harpoon gun after fully retracted
    [FormerlySerializedAs("_harpoonRemainsInHitObject")]
    [Tooltip("Toggle for if the harpoon remains stuck in a hit object")]
    [SerializeField] private bool _doesHarpoonRemainInHitObject;
    [Tooltip("The projectile being fired")]
    [SerializeField] private GameObject _harpoonPrefab; // Prefab of the harpoon

    private static HarpoonProjectileMovement[] _harpoonSpearPool;
    private int _harpoonPoolCounter;

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

    private bool _isFocusButtonHeld;

    private float _currentFocusAccuracy;

    private float _focusProgress;
    private EFocusState _currentFocusState;

    private Coroutine _focusUnfocusCoroutine;

    [Space]
    [Header("Harpoon Functionality Dependencies")]
    [Tooltip("Transform of whatever the cameras rotation is. Probably the cinemachine camera object")]
    [SerializeField] private Transform _playerLookDirection;
    [Tooltip("Transform on the end of the harpoon gun (wherever the harpoon comes out of)")]
    [SerializeField] private Transform _harpoonTip;
    [Tooltip("The harpoon object on the gun. Disappears and reappears to indicate whether player has shot ready")]
    [SerializeField] private GameObject _harpoonOnGun;
    [Tooltip("Layers the launched harpoon can not hit")]
    [SerializeField] private LayerMask _excludeLayers;
    [Tooltip("The input action for shooting")]
    [SerializeField] private InputActionReference _harpoonShoot;
    [Tooltip("The input action for focusing")]
    [SerializeField] private InputActionReference _harpoonFocus;

    [Tooltip("The amount of harpoons in the object pool")]
    private static readonly int _harpoonPoolingAmount = 5;

    [Space]
    [Header("Camera Shake Values")]
    [Tooltip("Recoil Intensity Shake")]
    [SerializeField] private float _recoilCameraShakeIntensity = 5f;
    [Tooltip("Recoil time Shake")]
    [SerializeField] private float _recoilCameraShakeTime = 0.05f;

    [Space]
    [Header("Animation")]
    [Tooltip("name of shoot animation")]
    [SerializeField] private string _harpoonShootTrigger = "shoot";
    [Tooltip("name of retract animation")]
    [SerializeField] private string _harpoonRetractTrigger = "drawBack";

    private PlayerInputMap _playerInputMap;

    public static HarpoonGun Instance;

    #endregion

    #region Setup
    private void Awake()
    {
        CheckSingletonInstance();

        CreateInitialHarpoonPool();

        SubscribeToEvents();

        StartCoroutine(HarpoonCameraOrientation());
    }

    /// <summary>
    /// Confirms whether this asset exists as a singleton.
    /// </summary>
    private void CheckSingletonInstance()
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
    /// Subscribes to all needed events
    /// </summary>
    private void SubscribeToEvents()
    {
        TimeManager.Instance.GetOnGamePauseEvent().AddListener(StartUnfocusingHarpoon);
    }

    /// <summary>
    /// Unsubscribes to all needed event
    /// </summary>
    private void UnsubscribeToEvents()
    {
        TimeManager.Instance.GetOnGamePauseEvent().RemoveListener(StartUnfocusingHarpoon);
    }

    /// <summary>
    /// Unsubscribes to events on destruction
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
    #endregion

    #region Input
    
    /// <summary>
    /// sets up the button for shooting
    /// </summary>
    public void SubscribeInput()
    {
        _harpoonShoot.action.performed += FireHarpoon;

        _harpoonFocus.action.started += FocusButtonHeld;
        _harpoonFocus.action.canceled += FocusButtonReleased;
    }

    /// <summary>
    /// disables shoot button
    /// </summary>
    public void UnsubscribeInput()
    {
        _harpoonShoot.action.performed -= FireHarpoon;

        _harpoonFocus.action.started -= FocusButtonHeld;
        _harpoonFocus.action.canceled -= FocusButtonReleased;
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

        HarpoonProjectileMovement currentHarpoon = GetNextHarpoonInObjectPool();

        currentHarpoon.gameObject.SetActive(true);

        currentHarpoon.LaunchHarpoon(_harpoonTip.transform.position, GetHarpoonDirectionWithFocus());

        _harpoonFiringState = EHarpoonFiringState.Firing;

        VfxManager.Instance.GetMuzzleSmokeVfx().PlayNextVfxInPool(BoatMover.Instance.transform, 
            transform.position, transform.rotation);

        ResetFocus();

        // Personally I think the projectile should be the same as the object on the visual as the gun itself, 
        // but that's a discussion for a later day
        _harpoonOnGun.SetActive(false);

        //Camera shake
        CinemachineShake.Instance.ShakeCamera(_recoilCameraShakeIntensity, _recoilCameraShakeTime);

        PlayerManager.Instance.InvokeOnHarpoonFiredEvent();

        RuntimeSfxManager.APlayOneShotSFX?
            .Invoke(FmodSfxEvents.Instance.PlayerTookDamage, gameObject.transform.position);

        StartReloadProcess();
    }
    
    #endregion

    #region Reloading
    /// <summary>
    /// Start the process of reloading
    /// </summary>
    private void StartReloadProcess()
    {
        _harpoonFiringState = EHarpoonFiringState.Reloading;

        StartCoroutine(ReloadHarpoon());

        RuntimeSfxManager.APlayOneShotSFX?
            .Invoke(FmodSfxEvents.Instance.PlayerTookDamage, gameObject.transform.position);
    }

    /// <summary>
    /// The process in which the harpoon is reloaded
    /// </summary>
    private IEnumerator ReloadHarpoon()
    {
        float reloadTimeRemaining = _reloadTime;
        while(reloadTimeRemaining > 0)
        {
            reloadTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        HarpoonFullyReloaded();
    }

    /// <summary>
    /// Called when the weapon is completely reloaded
    /// Resets the harpoon to be fired again
    /// </summary>
    private void HarpoonFullyReloaded()
    {
        _harpoonFiringState = EHarpoonFiringState.Ready;
        _harpoonOnGun.SetActive(true);

        if(_isFocusButtonHeld)
        {
            StartFocusingHarpoon();
        }

        PlayerManager.Instance.InvokeOnHarpoonReloadedEvent();
    }
    #endregion

    #region Focusing
    
    /// <summary>
    /// Called when the focus button begins to be held down
    /// </summary>
    /// <param name="context"></param>
    private void FocusButtonHeld(InputAction.CallbackContext context)
    {
        _isFocusButtonHeld = true;

        if (_harpoonFiringState == EHarpoonFiringState.Ready)
        {
            StartFocusingHarpoon();
        }
    }

    /// <summary>
    /// Called when the focus button is released
    /// </summary>
    /// <param name="context"></param>
    private void FocusButtonReleased(InputAction.CallbackContext context)
    {
        _isFocusButtonHeld = false;

        if (_harpoonFiringState == EHarpoonFiringState.Ready)
        {
            StartUnfocusingHarpoon();
        }
    }

    /// <summary>
    /// Starts focusing the weapon
    /// </summary>
    private void StartFocusingHarpoon()
    {
        _currentFocusState = EFocusState.Focusing;

        StopCurrentFocusCoroutine();
        _focusUnfocusCoroutine = StartCoroutine(FocusProcess());

        PlayerManager.Instance.InvokeOnHarpoonFocusStartEvent();
    }

    /// <summary>
    /// Stops the focusing of the weapon
    /// </summary>
    private void StartUnfocusingHarpoon()
    {
        _currentFocusState = EFocusState.Unfocusing;

        StopCurrentFocusCoroutine();
        _focusUnfocusCoroutine = StartCoroutine(UnfocusProcess());

        PlayerManager.Instance.InvokeOnHarpoonFocusEndEvent();
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

    /// <summary>
    /// Called when the weapon focus is at 100%
    /// Invokes needed events and makes sure values are correct
    /// </summary>
    private void FocusMax()
    {
        PlayerManager.Instance.InvokeOnHarpoonFocusMaxEvent();

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

        ResetFocus();
    }

    /// <summary>
    /// Resets focus back to 0
    /// </summary>
    private void ResetFocus()
    {
        StopCurrentFocusCoroutine();
        _focusProgress = 0;
        CalculateCurrentFocusAccuracy();
        _currentFocusState = EFocusState.None;
    }

    /// <summary>
    /// Determines what the current focus accuracy is based on the focus progress
    /// </summary>
    private void CalculateCurrentFocusAccuracy()
    {
        //This function is currently being done every time the focus progress is updated,
        //  you could argue that it should only be done in GetHarpoonDirectionWithFocus.
        //Currently, the only time you need to know the current accuracy is when you shoot, but if that were
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
        Vector3 endDir = (_playerLookDirection.forward * _maxDistance + transform.position 
            - _harpoonTip.transform.position).normalized;

        //Multiplies the direction the player is looking by a random variance scaled by current focus
        return endDir + Random.insideUnitSphere * _currentFocusAccuracy;
    }
    
    #endregion

    #region Harpoon Object Pool
    /// <summary>
    /// Creates all harpoon projectiles in the object pool to avoid needing to spawn more later
    /// </summary>
    private void CreateInitialHarpoonPool()
    {
        if(_harpoonSpearPool != null) { return; }

        //Sets the size of the object to pool to be determined based on the _harpoonPoolingAmount
        _harpoonSpearPool = new HarpoonProjectileMovement[_harpoonPoolingAmount];
        //Iterate for each space in the harpoon pool
        for(int i = 0; i < _harpoonPoolingAmount; i++)
        {
            //Spawn the new harpoon
            HarpoonProjectileMovement newestHarpoon = Instantiate(_harpoonPrefab, 
                _playerLookDirection.position, Quaternion.identity).GetComponent<HarpoonProjectileMovement>();
            //Adds the new harpoon to the pool
            _harpoonSpearPool[i] = newestHarpoon;
            ObjectPoolingParent.Instance.InitiallyAddObjectToPool(newestHarpoon.gameObject);
            newestHarpoon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the next object in the harpoon object pool
    /// </summary>
    /// <returns></returns>
    private HarpoonProjectileMovement GetNextHarpoonInObjectPool()
    {
        int previousPoolValue = _harpoonPoolCounter;
        _harpoonPoolCounter++;
        if(_harpoonPoolCounter >= _harpoonPoolingAmount)
        {
            _harpoonPoolCounter = 0;
        }

        return _harpoonSpearPool[previousPoolValue];
    }
    #endregion

    #region General

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
    public float GetHarpoonProjectileSpeed() => _fireSpeed;
    public float GetHarpoonMaxDistance() => _maxDistance;
    public LayerMask GetHarpoonExcludeLayers() => _excludeLayers;
    public bool GetDoesHarpoonRemainsInObject() => _doesHarpoonRemainInHitObject;
    public Transform GetHarpoonTip() => _harpoonTip;

    /// <summary>
    /// The focus accuracy (or potential deviation) of the harpoon.
    /// </summary>
    /// <returns>Deviation range of shots. Higher numbers mean more spread.</returns>
    public float GetCurrentFocusAccuracy()
    {
        return _currentFocusAccuracy;
    }

    /// <summary>
    /// The maximum inaccuracy of the harpoon gun.
    /// </summary>
    /// <returns>The initial starting inaccuracy of the harpoon gun.</returns>
    public float GetFocusStartingInaccuracy()
    {
        return _focusStartingInaccuracy;
    }

    /// <summary>
    /// The state of focusing.
    /// </summary>
    /// <returns>The current state of focusing.</returns>
    public EFocusState GetCurrentFocusState()
    {
        return _currentFocusState;
    }

    /// <summary>
    /// Whether a harpoon is ready to be fired or not.
    /// </summary>
    /// <returns>Current firing state of the player's harpoon.</returns>
    public EHarpoonFiringState GetHarpoonFiringState()
    {
        return _harpoonFiringState;
    }

    #endregion
}