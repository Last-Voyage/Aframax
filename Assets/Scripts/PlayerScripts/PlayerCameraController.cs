/******************************************************************************
// File Name:       PlayerCameraController.cs
// Author:          Andrew Stapay
// Contributor      Ryan Swanson, Adam Garwacki, Miles Rogers
// Creation Date:   September 19, 2024
//
// Description:     Implementation of the basic camera control for a player 
//                  character. This script takes input from the mouse and
//                  allows the Main Camera to rotate in the scene.
******************************************************************************/
using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(CinemachineVirtualCamera))]

/// <summary>
/// A class that controls the camera attached to the player.
/// Can be expanded upon to do whatever we need with the camera.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    public static PlayerCameraController Instance;

    [SerializeField] private GameObject _playerVisuals;

    // Variables for zooming in relation to harpoon gun focus
    [Tooltip("The camera's FOV at a full zoom, derived from focusing all the way.")]
    [SerializeField] private float _fullyZoomedFOV;
    private float _defaultFOV;
    private float _rangeOfFOV;

    // Variables for the Virtual Camera
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTransposer _transposer;
    private CinemachinePOV _cinemachinePOV;

    // Variables that relate to the camera's coroutines
    private Coroutine _cameraCoroutine;
    private Coroutine _walkingSwayCoroutine;

    private bool _walkingSwayStarted = false;

    // Variables for boat sway
    [Space]
    [SerializeField, Range(0f, 10f)] private float _boatSwaySpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float _boatSwayIntensity = 5f;
    private const float _BASE_BOAT_SWAY_SPEED = 0.005f;
    private const float _BOAT_SWAY_SPEED_LIMITER = 5f;
    private const float _BOAT_SWAY_INTENSITY_LIMITER = 20f;
    private float _currentBoatSwayChange = 0f;

    // Variables for movement sway
    [Space]
    [Header("Movement Sway")]
    [SerializeField] private GameObject _harpoonGun;
    private Animator _harpoonAnimator;

    [Space]
    [SerializeField, Range(0f, 10f)] private float _horizontalMovementSwaySpeed = 5f; 
    [SerializeField, Range(0f, 10f)] private float _horizontalMovementSwayIntensity = 5f;
    [SerializeField] private AnimationCurve _horizontalMovementSwayCurve;
    
    [Space]
    [SerializeField, Range(0f, 10f)] private float _verticalMovementSwaySpeed = 5f;
    [SerializeField] private AnimationCurve _verticalMovementSwayCurve;
    
    private const string _IDLE_ANIMATION = "harpoonIdle";
    private const string _START_SPRINT_ANIMATION = "StartSprint";
    private const string _SPRINT_ANIMATION = "Sprint";
    private const string _END_SPRINT_ANIMATION = "EndSprint";
    private const float _BASE_MOVEMENT_SWAY_SPEED = 0.00005f;
    private const float _BASE_VERTICAL_MOVEMENT_SWAY_SPEED = 0.0005f;
    private const float _BASE_MOVEMENT_SWAY_INTENSITY = 0.004f;
    private bool _movementSwayRight = true;
    // Cached variables for movement sway
    private float _xSway;
    private float _ySway;

    private float currentSwayDistance;
    private float _swayDistanceLimit;
    private float _verticalSwayDistanceLimit;

    // Variables for pullback
    [Space]
    [SerializeField] private float _jumpscareTime = 0.1f;
    [SerializeField, Range(0f, 10f)] private float _jumpscareIntensity = 5f;

    // Variables for harpoon turning
    [SerializeField, Range(0f, 10f)] private float _harpoonFollowTime = 5f;
    private const float _BASE_FOLLOW_TIME = 0.01f;
    private float _harpoonHorizontalVelocity;
    private float _harpoonVerticalVelocity;

    [Space]
    [SerializeField] private float _timeToReturnToMaxSpeed;
    [SerializeField] private float _delayToReturnToMaxSpeed;
    private WaitForSeconds _delayToMaxSpeedWait;
    private Coroutine _cameraSpeedReturnCoroutine;
    private Coroutine _stopSwayCoroutine;

    // Cached variables
    private WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
    private Transform _playerTransform;
    private Transform _cameraTransform;
    private Transform _harpoonTransform;

    private bool _isReticleFullyZoomed;

    public Vector2 StoredSensitivity;

    private CinemachineBrain _cinemachineBrain;

    private InputAction _playerMovement;

    /// <summary>
    /// Whether the reticle is visually fully shrunken or not.
    /// </summary>
    public bool IsReticleFullyZoomed
    {
        set { _isReticleFullyZoomed = value; }
    }

    public CinemachineVirtualCamera PlayerVirtualCamera { get => _virtualCamera; set => _virtualCamera = value; }

    /// <summary>
    /// Performs any needed set up before the first frame
    /// </summary>
    public void CameraSetup()
    {
        EstablishInstance();

        // Get the Virtual Camera component and start the coroutine
        InitializeCamera();

        InitializeFOVDifference();

        _playerTransform = _playerVisuals.transform;
        _harpoonTransform = _harpoonGun.transform;
        
        _swayDistanceLimit = _BASE_MOVEMENT_SWAY_INTENSITY * _horizontalMovementSwayIntensity;
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
    /// Initialize the camera variable.
    /// </summary>
    private void InitializeCamera()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _cinemachinePOV = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();

        _harpoonAnimator = _harpoonGun.GetComponent<Animator>();
        _cameraTransform = Camera.main.transform;

        _delayToMaxSpeedWait = new WaitForSeconds(_delayToReturnToMaxSpeed);
        InitializeStoredSensitivity();
    }

    /// <summary>
    /// Sets the stored sensitivity to its default values
    /// </summary>
    private void InitializeStoredSensitivity()
    {
        StoredSensitivity = new Vector2(_cinemachinePOV.m_HorizontalAxis.m_MaxSpeed,
            _cinemachinePOV.m_VerticalAxis.m_MaxSpeed);
    }

    /// <summary>
    /// Resets the value of the sensitivity to its stored value
    /// </summary>
    private void ResetToStoredSensitivity()
    {
        SetCinemachineSpeed(StoredSensitivity.x, StoredSensitivity.y);
    }

    /// <summary>
    /// Initializes the differences between the camera's base FOV and the FOV it
    /// should have when fully focused.
    /// </summary>
    private void InitializeFOVDifference()
    {
        _defaultFOV = _virtualCamera.m_Lens.FieldOfView;
        _rangeOfFOV = _defaultFOV - _fullyZoomedFOV;
    }

    /// <summary>
    /// Camera coroutine
    /// This will perpetually call the camera-moving method until disabled
    /// </summary>
    private IEnumerator MoveCamera()
    {
        while (true)
        {
            AdjustPlayerRotation();
            AdjustHarpoonRotation();
            BoatSway();

            yield return null;
        }
    }

    /// <summary>
    /// Adjusts the player model such that it faces the same way as the camera.
    /// Used to assist movement.
    /// </summary>
    private void AdjustPlayerRotation()
    {
        // Cinemachine actually manipulates the Main Camera itself
        // By getting the rotation of the Main Camera, we can rotate our character
        _playerTransform.rotation = Quaternion.Euler(0,_cameraTransform.eulerAngles.y,0);
    }

    /// <summary>
    /// Adjusts the harpoon's rotation such that it follows the camera movement
    /// Readjusts the harpoon for animations as well.
    /// </summary>
    private void AdjustHarpoonRotation()
    {
        if (!_harpoonAnimator.IsUnityNull())
        {
            string currentAnimationState = _harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (currentAnimationState == _IDLE_ANIMATION || currentAnimationState == _START_SPRINT_ANIMATION ||
                currentAnimationState == _SPRINT_ANIMATION || currentAnimationState == _END_SPRINT_ANIMATION)
            {
                UnchildHarpoon();
            }
            else
            {
                ChildHarpoon();
            }
        }
    }

    /// <summary>
    /// Simulates the swaying of the boat
    /// </summary>
    private void BoatSway()
    {
        if (!_virtualCamera.IsUnityNull())
        {
            // Update our change from the original camera position
            _currentBoatSwayChange += _BASE_BOAT_SWAY_SPEED * _boatSwaySpeed / _BOAT_SWAY_SPEED_LIMITER;

            // We'll move the camera based on a sine wave (starts at 0, oscillates between 1 and -1)
            float newY = Mathf.Sin(_currentBoatSwayChange) * _boatSwayIntensity / _BOAT_SWAY_INTENSITY_LIMITER;
            _transposer.m_FollowOffset = new Vector3(_transposer.m_FollowOffset.x, newY, _transposer.m_FollowOffset.z);

            // We don't want our sway change to get insanely large
            // To fix this, we'll make sure it stays within the bounds [0, 2*pi]
            _currentBoatSwayChange = _currentBoatSwayChange % (2 * Mathf.PI);
        }
    }

    /// <summary>
    /// Starts the walking sway coroutine
    /// </summary>
    /// <param name="playerMovement"> The InputAction associated with the player's movement for tracking </param>
    private void StartWalkingSway(InputAction playerMovement)
    {
        // Let's keep the movement input action here
        if (_playerMovement.IsUnityNull())
        {
            _playerMovement = playerMovement;
        }

        // Stop the camera returning coroutine
        if (_walkingSwayCoroutine != null)
        {
            _walkingSwayStarted = false;
            StopCoroutine(_walkingSwayCoroutine);
            _walkingSwayCoroutine = null;
        }

        // Start the walking sway
        if (!_walkingSwayStarted)
        {
            _walkingSwayStarted = true;
            _walkingSwayCoroutine = StartCoroutine(WalkingSway());
        }
    }

    /// <summary>
    /// Simulates walking sway, going left and right periodically
    /// </summary>
    private IEnumerator WalkingSway()
    {
        while (true)
        {
            Vector2 moveDir = _playerMovement.ReadValue<Vector2>();

            // We only really want to do movement sway if we are moving directly forward and the harpoon is idle
            // If we don't do this, this could lead to visual bugs
            
            // Adjusted the x calculation to be <= .2f so that the movement sway works on controller
            if (moveDir.y > 0 && Mathf.Abs(moveDir.x) <= .2f && 
                _harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _IDLE_ANIMATION)
            {
                // Stop resetting the camera if we are
                if (_stopSwayCoroutine != null)
                {
                    StopCoroutine(_stopSwayCoroutine);
                    _stopSwayCoroutine = null;
                }
                
                CalculateCurrentSwayDistance();

                // We would like to get the angle at which that camera is facing
                // So that we can move the harpoon accurately when the player turns
                float angle = _cameraTransform.localEulerAngles.y * Mathf.PI / 180f;

                // Movement Sway
                float newX = 0;
                float newY = 0;
                float newZ = 0;
                

                float distancePercent = currentSwayDistance / _swayDistanceLimit;
                
                float swaySpeedProgressMultiplier = _horizontalMovementSwayCurve.Evaluate(distancePercent);
                float verticalSwaySpeedProgressMultiplier = _verticalMovementSwayCurve.Evaluate(distancePercent);
                
                
                if (_movementSwayRight)
                {
                    newX = _harpoonTransform.localPosition.x +
                        (_BASE_MOVEMENT_SWAY_SPEED * _horizontalMovementSwaySpeed* swaySpeedProgressMultiplier) * Mathf.Cos(angle);
                    newZ = _harpoonTransform.localPosition.z +
                        (_BASE_MOVEMENT_SWAY_SPEED * _horizontalMovementSwaySpeed* swaySpeedProgressMultiplier) * Mathf.Sin(angle);
                }
                else
                {
                    newX = _harpoonTransform.localPosition.x -
                        (_BASE_MOVEMENT_SWAY_SPEED * _horizontalMovementSwaySpeed * swaySpeedProgressMultiplier) * Mathf.Cos(angle);
                    newZ = _harpoonTransform.localPosition.z -
                        (_BASE_MOVEMENT_SWAY_SPEED * _horizontalMovementSwaySpeed* swaySpeedProgressMultiplier) * Mathf.Sin(angle);
                }
                
                newY = (_BASE_VERTICAL_MOVEMENT_SWAY_SPEED * _verticalMovementSwaySpeed * verticalSwaySpeedProgressMultiplier);
                
                _harpoonTransform.localPosition = new Vector3(newX, newY, newZ);

                // If we reach the limit on our sway, switch directions

                CalculateCurrentSwayDistance();
                
                //print("Sway Percent" + currentSwayDistance / _swayDistanceLimit);
                if (currentSwayDistance >= _swayDistanceLimit)
                {
                    _movementSwayRight = !_movementSwayRight;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// Calculate the current sway distance from the original point
    /// </summary>
    void CalculateCurrentSwayDistance()
    {
        Vector3 tempVector =
            new Vector3(_harpoonTransform.localPosition.x, 0, _harpoonTransform.localPosition.z);
        currentSwayDistance = Vector3.Distance(tempVector, Vector3.zero);
    }

    /// <summary>
    /// Stops the walking sway coroutine and resets the camera
    /// </summary>
    private void StopWalkingSway()
    {
        // Stop walking sway
        if (_walkingSwayCoroutine != null)
        {
            StopCoroutine(_walkingSwayCoroutine);
            _walkingSwayCoroutine = null;
        }

        // Return camera to original position
        _stopSwayCoroutine = StartCoroutine(ReturnCameraFromWalking());
    }

    /// <summary>
    /// Returns the camera to its original position from the walking sway motion
    /// </summary>
    private IEnumerator ReturnCameraFromWalking()
    {
        if (_harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _IDLE_ANIMATION)
        {
            while (_harpoonTransform.localPosition != Vector3.zero)
            {
                // Slowly move the harpoon back to position
                _harpoonTransform.localPosition = Vector3.MoveTowards(_harpoonTransform.localPosition,
                    Vector3.zero, _BASE_MOVEMENT_SWAY_SPEED * _horizontalMovementSwaySpeed);

                yield return null;
            }
        }
        else
        {
            _harpoonTransform.localPosition = Vector3.zero;
        }

        // I'm deciding that our main character is right footed
        _movementSwayRight = true;
        _walkingSwayStarted = false;

        // Prevents camera sway from getting duplicated
        if (_walkingSwayCoroutine != null)
        {
            StopCoroutine(_walkingSwayCoroutine);
            _walkingSwayCoroutine = null;
        }

        // But wait, if we're still in motion, then we want to restart the walking sway
        if (_playerMovement != null &&
            _playerMovement.ReadValue<Vector2>() != Vector2.zero)
        {
            _walkingSwayStarted = true;
            _walkingSwayCoroutine = StartCoroutine(WalkingSway());
        }
    }

    /// <summary>
    /// Activates or deactivates the camera coroutine based on the input boolean
    /// Used when the OnCameraMovementToggled Action is invoked
    /// </summary>
    /// <param name="change"> Determines if the camera movement should be turned on or off </param>
    private void ToggleCameraMovement(bool change)
    {
        if (change)
        {
            _cameraCoroutine = StartCoroutine(MoveCamera());
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            _cameraSpeedReturnCoroutine = StartCoroutine(ReturnCameraSpeed());
        }
        else
        {
            if (_cameraCoroutine != null)
            {
                StopCoroutine(_cameraCoroutine);
            }
            if(_cameraSpeedReturnCoroutine != null)
            {
                StopCoroutine(_cameraSpeedReturnCoroutine);
            }
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            SetCinemachineSpeed(0, 0);
        }
    }

    /// <summary>
    /// Returns the camera to its default speed
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnCameraSpeed()
    {
        //ResetToStoredSensitivity();
        float returnSpeedProgress = 0;
        yield return _delayToMaxSpeedWait;
        while (returnSpeedProgress < 1)
        {
            returnSpeedProgress+= Time.deltaTime/_timeToReturnToMaxSpeed;
            SetCinemachineSpeed(Mathf.Lerp(0, StoredSensitivity.x, returnSpeedProgress),
                Mathf.Lerp(0, StoredSensitivity.y, returnSpeedProgress));
            yield return null;
        }
    }

    /// <summary>
    /// Sets the speed of the cinemachine camera
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void SetCinemachineSpeed(float x, float y)
    {
        _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed =x;
        _cinemachinePOV.m_VerticalAxis.m_MaxSpeed =y;
    }

    /// <summary>
    /// Starts the jumpscare pullback camera motion
    /// </summary>
    private void JumpscarePullback()
    {
        CinemachineShake.Instance.ShakeCamera(_jumpscareIntensity, _jumpscareTime, false);
    }

    /// <summary>
    /// Adjusts the zoom of the harpoon gun in relation to the player's focus inputs.
    /// </summary>
    /// <param name="focusProgress">As a percentage, how much the player has focused. 0 is 0%, 1 is 100%.</param>
    public void AdjustZoom(float focusProgress)
    {
        if (!_isReticleFullyZoomed)
        {
            _virtualCamera.m_Lens.FieldOfView = _defaultFOV - (_rangeOfFOV * focusProgress);
        }
    }

    /// <summary>
    /// Resets the zoom of the camera to its default FOV.
    /// </summary>
    public void ResetZoom()
    {
        _virtualCamera.m_Lens.FieldOfView = _defaultFOV;
    }

    /// <summary>
    /// Sets the harpoon to be the child of the main camera
    /// Used during the logic of cinematics and animations
    /// </summary>
    public void ChildHarpoon()
    {
        // Because of the harpoon's animations, we need the harpoon to attach to the Main Camera
        // to keep its rotation when it's not idle
        _harpoonTransform.SetParent(_cameraTransform, true);

        // Let's reset the position and rotation too, just in case
        _harpoonTransform.localRotation = Quaternion.identity;
        _harpoonTransform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Sets the harpoon to be the child of this object
    /// Used during the logic of cinematics and animations
    /// </summary>
    public void UnchildHarpoon()
    {
        // Let's make sure the harpoon is set to the PlayerCamera as its parent
        // This is because we want to ignore the Main Camera's current rotation so we can just do it ourselves
        _harpoonTransform.SetParent(this.transform, true);

        // Get new angles for the harpoon
        // We do this by getting the current rotation for the harpoon and putting it through this
        // SmoothDampAngle function, which is super intuitive and makes the movement clean
        float newHoriAngle = Mathf.SmoothDampAngle(_harpoonTransform.localEulerAngles.y,
            _cameraTransform.localEulerAngles.y, ref _harpoonHorizontalVelocity,
            _harpoonFollowTime * _BASE_FOLLOW_TIME);
        float newVertAngle = Mathf.SmoothDampAngle(_harpoonTransform.localEulerAngles.x,
            _cameraTransform.localEulerAngles.x, ref _harpoonVerticalVelocity,
            _harpoonFollowTime * _BASE_FOLLOW_TIME);

        // Set new angles for the harpoon
        _harpoonTransform.localRotation = Quaternion.Euler(newVertAngle, newHoriAngle, 0);
    }

    /// <summary>
    /// Stops the camera movement coroutine
    /// </summary>
    public void StopAutoCameraMovement()
    {
        StopCoroutine(_cameraCoroutine);
        _cameraCoroutine = null;
    }

    /// <summary>
    /// Restarts the camera movement coroutine
    /// </summary>
    public void RestartAutoCameraMovement()
    {
        if (_cameraCoroutine.IsUnityNull())
        {
            _cameraCoroutine = StartCoroutine(MoveCamera());
        }
    }

    /// <summary>
    /// Called when this component is enabled.
    /// Used to assign various actions to listeners
    /// </summary>
    private void OnEnable()
    {
        PlayerManager.Instance.GetOnMovementStartEvent().AddListener(StartWalkingSway);
        PlayerManager.Instance.GetOnMovementEndEvent().AddListener(StopWalkingSway);
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(StopWalkingSway);
        CameraManager.Instance.GetOnJumpscareEvent().AddListener(JumpscarePullback);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(StopAutoCameraMovement);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(UnchildHarpoon);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(StopWalkingSway);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(ResetZoom);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(HideHarpoonGun);
        CameraManager.Instance.GetOnCinematicEndEvent().AddListener(RestartAutoCameraMovement);
        CameraManager.Instance.GetOnCinematicEndEvent().AddListener(ShowHarpoonGun);
    }

    /// <summary>
    /// Called when this component is disabled.
    /// Used to unassign various actions to listeners
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetOnMovementStartEvent().RemoveListener(StartWalkingSway);
        PlayerManager.Instance.GetOnMovementEndEvent().RemoveListener(StopWalkingSway);
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StopWalkingSway);
        CameraManager.Instance.GetOnJumpscareEvent().RemoveListener(JumpscarePullback);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(StopAutoCameraMovement);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(UnchildHarpoon);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(StopWalkingSway);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(ResetZoom);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(HideHarpoonGun);
        CameraManager.Instance.GetOnCinematicEndEvent().RemoveListener(RestartAutoCameraMovement);
        CameraManager.Instance.GetOnCinematicEndEvent().RemoveListener(ShowHarpoonGun);
    }

    /// <summary>
    /// Enables all camera player input
    /// </summary>
    public void SubscribeInput()
    {
        ToggleCameraMovement(true);
    }

    /// <summary>
    /// Disables all camera player input
    /// </summary>
    public void UnsubscribeInput()
    {
        ToggleCameraMovement(false);
    }

    /// <summary>
    /// Hides the harpoon gun model
    /// Used for cinematics
    /// </summary>
    private void HideHarpoonGun()
    {
        _harpoonGun.transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the harpoon gun model
    /// Used for cinematics
    /// </summary>
    public void ShowHarpoonGun()
    {
        _harpoonGun.transform.GetChild(0).gameObject.SetActive(true);
    }
}
