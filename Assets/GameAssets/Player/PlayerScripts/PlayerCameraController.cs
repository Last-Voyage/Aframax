/******************************************************************************
// File Name:       PlayerCameraController.cs
// Author:          Andrew Stapay
// Contributor      Ryan Swanson
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

    // Variables for the Virtual Camera
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineTransposer _transposer;

    // Variables that relate to the camera's coroutines
    private Coroutine _cameraCoroutine;
    private Coroutine _walkingSwayCoroutine;

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
    [SerializeField] private GameObject _harpoonGun;
    private Animator _harpoonAnimator;
    [SerializeField, Range(0f, 10f)] private float _movementSwaySpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float _movementSwayIntensity = 5f;
    private const string _IDLE_ANIMATION = "harpoonIdle";
    private const float _BASE_MOVEMENT_SWAY_SPEED = 0.00005f;
    private const float _BASE_MOVEMENT_SWAY_INTENSITY = 0.004f;
    private bool _movementSwayRight = true;

    // Variables for pullback
    [Space]
    [SerializeField] private float _jumpscareTime = 0.1f;
    [SerializeField, Range(0f, 10f)] private float _jumpscareIntensity = 5f;

    // Variables for harpoon turning
    [SerializeField, Range(0f, 10f)] private float _harpoonFollowTime = 5f;
    private const float _BASE_FOLLOW_TIME = 0.01f;
    private float _harpoonHorizontalVelocity;
    private float _harpoonVerticalVelocity;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized
    /// and to start the coroutine
    /// </summary>
    void Start()
    {
        EstablishInstance();

        // Get the Virtual Camera component and start the coroutine
        InitializeCamera();
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

        _harpoonAnimator = _harpoonGun.GetComponent<Animator>();
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
        _playerVisuals.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// Adjusts the harpoon's rotation such that it follows the camera movement
    /// Readjusts the harpoon for animations as well.
    /// </summary>
    private void AdjustHarpoonRotation()
    {
        if (!_harpoonAnimator.IsUnityNull())
        {
            if (_harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _IDLE_ANIMATION)
            {
                // Let's make sure the harpoon is set to the PlayerCamera as its parent
                // This is because we want to ignore the Main Camera's current rotation so we can just do it ourselves
                _harpoonGun.transform.SetParent(this.transform, true);

                // Get new angles for the harpoon
                // We do this by getting the current rotation for the harpoon and putting it through this
                // SmoothDampAngle function, which is super intuitive and makes the movement clean
                float newHoriAngle = Mathf.SmoothDampAngle(_harpoonGun.transform.localEulerAngles.y,
                    Camera.main.transform.localEulerAngles.y, ref _harpoonHorizontalVelocity, 
                    _harpoonFollowTime * _BASE_FOLLOW_TIME);
                float newVertAngle = Mathf.SmoothDampAngle(_harpoonGun.transform.localEulerAngles.x,
                    Camera.main.transform.localEulerAngles.x, ref _harpoonVerticalVelocity, 
                    _harpoonFollowTime * _BASE_FOLLOW_TIME);

                // Set new angles for the harpoon
                _harpoonGun.transform.localRotation = Quaternion.Euler(newVertAngle, newHoriAngle, 0);
            }
            else
            {
                // Because of the harpoon's animations, we need the harpoon to attach to the Main Camera
                // to keep its rotation when it's not idle
                _harpoonGun.transform.SetParent(Camera.main.transform, true);

                // Let's reset the rotation too, just in case
                _harpoonGun.transform.localRotation = Quaternion.identity;
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
        // Stop the camera returning coroutine
        if (_walkingSwayCoroutine != null)
        {
            StopCoroutine(_walkingSwayCoroutine);
        }

        // Start the walking sway
        _walkingSwayCoroutine = StartCoroutine(WalkingSway(playerMovement));
    }

    /// <summary>
    /// Simulates walking sway, going left and right periodically
    /// </summary>
    /// <param name="playerMovement"> The InputAction associated with the player's movement for tracking </param>
    private IEnumerator WalkingSway(InputAction playerMovement)
    {
        Coroutine stopSwayCoroutine = null;

        while (true)
        {
            Vector2 moveDir = playerMovement.ReadValue<Vector2>();

            // We only really want to do movement sway if we are moving directly forward and the harpoon is idle
            // If we don't do this, this could lead to visual bugs
            if (moveDir.y > 0 && moveDir.x == 0 && 
                _harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _IDLE_ANIMATION)
            {
                // Stop resetting the camera if we are
                if (stopSwayCoroutine != null)
                {
                    StopCoroutine(stopSwayCoroutine);
                }

                // Movement Sway
                float newX = 0;
                if (_movementSwayRight)
                {
                    newX = _harpoonGun.transform.localPosition.x +
                        (_BASE_MOVEMENT_SWAY_SPEED * _movementSwaySpeed);
                }
                else
                {
                    newX = _harpoonGun.transform.localPosition.x -
                        (_BASE_MOVEMENT_SWAY_SPEED * _movementSwaySpeed);
                }
                _harpoonGun.transform.localPosition = new Vector3(newX, 0, 0);

                // If we reach the limit on our sway, switch directions
                float swayDistanceLimit = _BASE_MOVEMENT_SWAY_INTENSITY * _movementSwayIntensity;
                float currentSwayDistance = Vector3.Distance(_harpoonGun.transform.localPosition, Vector3.zero);
                if (currentSwayDistance >= swayDistanceLimit)
                {
                    _movementSwayRight = !_movementSwayRight;
                }
            }
            else
            {
                // If we aren't moving directly forward, let's just reset the camera
                if (_transposer.m_FollowOffset.x != 0)
                {
                    stopSwayCoroutine = StartCoroutine(ReturnCameraFromWalking());
                }
            }

            yield return null;
        }
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
        }
        // Return camera to original position
        _walkingSwayCoroutine = StartCoroutine(ReturnCameraFromWalking());
    }

    /// <summary>
    /// Returns the camera to its original position from the walking sway motion
    /// </summary>
    private IEnumerator ReturnCameraFromWalking()
    {
        if (_harpoonAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == _IDLE_ANIMATION)
        {
            while (_harpoonGun.transform.localPosition != Vector3.zero)
            {
                // Slowly move the harpoon back to position
                _harpoonGun.transform.localPosition = Vector3.MoveTowards(_harpoonGun.transform.localPosition,
                    Vector3.zero, _BASE_MOVEMENT_SWAY_SPEED * _movementSwaySpeed);

                yield return null;
            }
        }
        else
        {
            _harpoonGun.transform.localPosition = Vector3.zero;
        }

        // I'm deciding that our main character is right footed
        _movementSwayRight = true;

        //Prevents camera sway from getting duplicated
        if (_walkingSwayCoroutine != null)
        {
            StopCoroutine(_walkingSwayCoroutine);
        }

        _walkingSwayCoroutine = null;
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
        }
        else
        {
            if (_cameraCoroutine != null)
            {
                StopCoroutine(_cameraCoroutine);
            }
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// Starts the jumpscare pullback camera motion
    /// </summary>
    private void JumpscarePullback()
    {
        CinemachineShake.Instance.ShakeCamera(_jumpscareIntensity, _jumpscareTime, false);
    }

    /// <summary>
    /// Called when this component is enabled.
    /// Used to assign various actions to listeners
    /// </summary>
    private void OnEnable()
    {
        PlayerManager.Instance.GetOnMovementStartEvent().AddListener(StartWalkingSway);
        PlayerManager.Instance.GetOnMovementEndEvent().AddListener(StopWalkingSway);
        CameraManager.Instance.GetOnJumpscareEvent().AddListener(JumpscarePullback);
    }

    /// <summary>
    /// Called when this component is disabled.
    /// Used to unassign various actions to listeners
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetOnMovementStartEvent().RemoveListener(StartWalkingSway);
        PlayerManager.Instance.GetOnMovementEndEvent().RemoveListener(StopWalkingSway);
        CameraManager.Instance.GetOnJumpscareEvent().RemoveListener(JumpscarePullback);
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
}
