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
    [SerializeField, Range(0f, 10f)] private float _movementSwaySpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float _movementSwayIntensity = 5f;
    private const float _BASE_MOVEMENT_SWAY_SPEED = 0.0025f;
    private const float _BASE_MOVEMENT_SWAY_INTENSITY = 0.25f;
    private const float _MOVEMENT_SWAY_SPEED_LIMITER = 5f;
    private const float _MOVEMENT_SWAY_INTENSITY_LIMITER = 5f;
    private bool _movementSwayRight = true;

    // Variables for pullback
    [Space]
    [SerializeField, Range(0f, 10f)] private float _pullbackSpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float _pullbackIntensity = 5f;
    private const float _PULLBACK_SPEED_LIMITER = 150f;
    private const float _PULLBACK_INTENSITY_LIMITER = 5f;

    // Variables for camera shake
    [Space]
    [SerializeField] private float _cameraShakeTime = 5f;
    [SerializeField, Range(0f, 10f)] private float _cameraShakeIntensity = 5f;

    // THIS IS TEMPORARY CODE THAT IS TO BE USED FOR TESTING PURPOSES
    // IF YOU ARE SEEING THIS MESSAGE, THEN STAPAY FORGOT TO REMOVE THIS
    // please remove it for me thanks :) - Stapay
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            CameraManager.Instance.InvokeOnJumpscare();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            CinemachineShake.Instance.ShakeCamera(_cameraShakeIntensity, _cameraShakeTime, true);
        }
    }

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
    /// Simulates the swaying of the boat
    /// </summary>
    private void BoatSway()
    {
        if (_virtualCamera != null)
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

            // We only really want to do movement sway if we are moving directly forward
            // If we don't do this, this could lead to visual bugs
            if (moveDir.y > 0 && moveDir.x == 0)
            {
                // Stop resetting the camera if we are
                if (stopSwayCoroutine != null)
                {
                    StopCoroutine(stopSwayCoroutine);
                }

                // Movement Sway
                if (_movementSwayRight)
                {
                    float newX = _transposer.m_FollowOffset.x +
                        (_BASE_MOVEMENT_SWAY_SPEED * _movementSwaySpeed / _MOVEMENT_SWAY_SPEED_LIMITER);
                    _transposer.m_FollowOffset = new Vector3(newX, _transposer.m_FollowOffset.y, 
                        _transposer.m_FollowOffset.z);
                }
                else
                {
                    float newX = _transposer.m_FollowOffset.x -
                        (_BASE_MOVEMENT_SWAY_SPEED * _movementSwaySpeed / _MOVEMENT_SWAY_SPEED_LIMITER);
                    _transposer.m_FollowOffset = new Vector3(newX, _transposer.m_FollowOffset.y,
                        _transposer.m_FollowOffset.z);
                }

                // If we reach the limit on our sway, switch directions
                float limit = _BASE_MOVEMENT_SWAY_INTENSITY * _movementSwayIntensity /
                    _MOVEMENT_SWAY_INTENSITY_LIMITER;
                if (_transposer.m_FollowOffset.x >= limit)
                {
                    _movementSwayRight = false;
                }
                else if (_transposer.m_FollowOffset.x <= -limit)
                {
                    _movementSwayRight = true;
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
            _walkingSwayCoroutine = null;
        }

        // Return camera to original position
        StartCoroutine(ReturnCameraFromWalking());
    }

    /// <summary>
    /// Returns the camera to its original position from the walking sway motion
    /// </summary>
    private IEnumerator ReturnCameraFromWalking()
    {
        // We only really changed our X value in this code, so let's return it
        while (_transposer.m_FollowOffset.x != 0)
        {
            Vector3 targetPos = new Vector3(0, _transposer.m_FollowOffset.y, 0);

            // Slowly move the camera back to position
            _transposer.m_FollowOffset = Vector3.MoveTowards(_transposer.m_FollowOffset, targetPos, 
                _BASE_MOVEMENT_SWAY_SPEED);

            yield return null;
        }

        // I'm deciding that our main character is right footed
        _movementSwayRight = true;
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
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            if (_cameraCoroutine != null)
            {
                StopCoroutine(_cameraCoroutine);
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// Starts the jumpscare pullback camera motion
    /// </summary>
    private void JumpscarePullback()
    {
        StartCoroutine(Pullback());
    }

    /// <summary>
    /// Performs the jumpscare pullback camera motion
    /// </summary>
    private IEnumerator Pullback()
    {
        print(_playerVisuals.transform.forward);
        // Let's do this based on a sine wave like we did for the boat sway
        // We'll start at pi so that the camera will move backwards
        float pullbackChange = Mathf.PI;

        while (pullbackChange < (2 * Mathf.PI))
        {
            // Update our pullback distance
            pullbackChange += Mathf.PI * _pullbackSpeed / _PULLBACK_SPEED_LIMITER;

            // Do the thing
            float newX = (Mathf.Sin(pullbackChange) * _pullbackIntensity / _PULLBACK_INTENSITY_LIMITER) * 
                _playerVisuals.transform.forward.x;
            float newZ = (Mathf.Sin(pullbackChange) * _pullbackIntensity / _PULLBACK_INTENSITY_LIMITER) * 
                _playerVisuals.transform.forward.z;
            _transposer.m_FollowOffset = new Vector3(newX, _transposer.m_FollowOffset.y, newZ);

            yield return null;
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
