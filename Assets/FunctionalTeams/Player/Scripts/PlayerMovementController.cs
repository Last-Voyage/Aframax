/******************************************************************************
// File Name:       PlayerMovementController.cs
// Author:          Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Implementation of the basic movement for a player character.
//                  This script takes input designated for movement from the
                    user and allows the player GameObject to move in the scene.
******************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]

/// <summary>
/// A class that handless the movement of the player character.
/// Follow the steps outlined in the documentation to set this up in the editor.
/// When attached to the player, use the WASD keys to move around.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    /// <summary>
    /// Variables that relate to the horizontal movement of the player
    /// </summary>
    [Header("Adjustable Speed")]
    [SerializeField] private float _playerMovementSpeed;

    [Space]
    [SerializeField] private float _focusSpeedSlowTime;
    [SerializeField] private AnimationCurve _focusMoveSpeedCurve;
    private float _currentFocusMoveSpeedMultiplier = 1;

    private Coroutine _focusSpeedCoroutine;

    // IMPORTANT: THIS CLASS ALSO TEMPORARILY CONTROLS THE CAMERA
    // REMOVE THIS WHEN WE IMPLEMENT A MORE DYNAMIC CAMERA
    /// <summary>
    /// Variables that relate to the rotation of the camera
    /// </summary>
    [Header("Set Player Camera Here")]
    [SerializeField] private Transform _playerCamera;
    [Header("Adjustable Mouse Sensitivities")]
    [Tooltip("X controls Left/Right\nY controls Up/Down")]
    [SerializeField] private float _mouseSensitivityX;
    [SerializeField] private float _mouseSensitivityY;

    /// <summary>
    /// _cameraXRotation is used to keep track of the current rotation
    /// of the camera. This is used while moving the mouse up and down
    /// </summary>
    private float _cameraXRotation;

    /// <summary>
    /// _CAMERA_CLAMP is a constant used to prevent the camera from starting
    /// to point behind the player while looking up and down. Set to 90 so that
    /// the camera may still look straight up or down
    /// </summary>
    private const float _CAMERA_CLAMP = 90;

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    // THE BELOW INPUT ACTIONS ARE USED FOR THE CAMERA
    private InputAction _mouseXInput;
    private InputAction _mouseYInput;
    private const string _MOVEMENT_INPUT_NAME = "Movement";
    private const string _MOUSE_X_INPUT_NAME = "MouseX";
    private const string _MOUSE_Y_INPUT_NAME = "MouseY";

    /// <summary>
    /// A variable to hold the Rigidbody
    /// </summary>
    private Rigidbody _rigidBody;

    /// <summary>
    /// Movement coroutine related variables
    /// </summary>
    public static event Action<bool> OnMovementToggled;
    private Coroutine _movementCoroutine;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized.
    /// </summary>
    void Start()
    {
        // Initialize non-serialized variables, input variables, and
        // the Rigidbody
        InitializeNonSerialized();
        InitializeInput();
        InitializeRigidbody();

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine("ResolveMovement");
    }

    /// <summary>
    /// Initializes all non-serialized private variables.
    /// </summary>
    private void InitializeNonSerialized()
    {
        _cameraXRotation = 0;
    }

    /// <summary>
    /// Initializes all input variables
    /// </summary>
    private void InitializeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();
        _movementInput = _playerInput.currentActionMap.FindAction(_MOVEMENT_INPUT_NAME);
        _mouseXInput = _playerInput.currentActionMap.FindAction(_MOUSE_X_INPUT_NAME);
        _mouseYInput = _playerInput.currentActionMap.FindAction(_MOUSE_Y_INPUT_NAME);
    }

    /// <summary>
    /// Initializes our Rigidbody for gravity
    /// </summary>
    private void InitializeRigidbody()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.freezeRotation = true;
    }

    /// <summary>
    /// Movement coroutine
    /// This will perpetually call the movement handling methods until disabled
    /// </summary>
    private IEnumerator ResolveMovement()
    {
        while(true)
        {
            HandleMovement();
            HandleCameraRotation();
            yield return null;
        }
    }

    /// <summary>
    /// Handles the general movement of the player
    /// Adds the horizontal and vertical movements to get the overall movement
    /// </summary>
    private void HandleMovement()
    {
        Vector3 horizontalMovement = HandleHorizontalMovement();
        Vector3 verticalMovement = HandleVerticalMovement();

        _rigidBody.velocity = horizontalMovement + verticalMovement;
    }

    /// <summary>
    /// Handler for horizontal movement based on input key presses
    /// </summary>
    private Vector3 HandleHorizontalMovement()
    {
        // Read the movement input
        Vector2 moveDir = _movementInput.ReadValue<Vector2>();

        // transform.right and transform.forward are vectors that point
        // in certain directions in the world
        // By manipulating them, we can move the character
        Vector3 newMovement = (transform.right * moveDir.x + transform.forward * moveDir.y)
            * _playerMovementSpeed * _currentFocusMoveSpeedMultiplier;

        // Move the player
        return newMovement;
    }

    /// <summary>
    /// Handles the force of gravity
    /// Can be expanded to include jumping later on
    /// </summary>
    private Vector3 HandleVerticalMovement()
    {
        return new Vector3(0, _rigidBody.velocity.y, 0);
    }

    /// <summary>
    /// THIS METHOD IS TEMPORARY UNTIL A CAMERA CONTROLLER IS CREATED
    /// Handles the rotation of the camera based on mouse movement
    /// </summary>
    private void HandleCameraRotation()
    {
        // First, rotate the player character based on
        // the horizontal movement of the mouse
        // We do this by manipulating Vector3.up, which is similar to how we
        // handled the horizontal movement
        transform.Rotate(Vector3.up, _mouseXInput.ReadValue<float>() * Time.deltaTime * _mouseSensitivityX);

        // Instead of rotating the player up and down, we will rotate the camera
        // Change the camera's rotation by reading the vertical mouse movement
        _cameraXRotation -= _mouseYInput.ReadValue<float>() * _mouseSensitivityY;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -_CAMERA_CLAMP, _CAMERA_CLAMP);

        // Create a target rotation for the camera based on the player's rotation
        // and the new rotation we just found
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = _cameraXRotation;
        _playerCamera.eulerAngles = targetRotation;
    }

    #region Harpoon Slowdown
    /// <summary>
    /// Will be reworked later to be private by using events, can't do so atm as 
    /// the player manager has been changed elsewhere and I don't want merge conflicts
    /// </summary>
    public void StartHarpoonSpeedSlowdown()
    {
        _focusSpeedCoroutine = StartCoroutine(HarpoonSpeedSlowdownProcess());
    }

    /// <summary>
    /// The process of slowing down the player while focusing
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarpoonSpeedSlowdownProcess()
    {
        float slowCompletion = 0;
        while (slowCompletion < 1)
        {
            //Increases the progress on slowdown
            slowCompletion += Time.deltaTime / _focusSpeedSlowTime;

            //Sets the current speed based on the animation graph
            _currentFocusMoveSpeedMultiplier = _focusMoveSpeedCurve.Evaluate(slowCompletion);

            yield return null;
        }
    }

    /// <summary>
    /// Will be reworked later to be private by using events, can't do so atm as the player manager has been changed
    /// </summary>
    public void StopHarpoonSpeedSlowdown()
    {
        _currentFocusMoveSpeedMultiplier = 1;
        StopCoroutine(_focusSpeedCoroutine);
    }
    #endregion

    /// <summary>
    /// Activates or deactivates the movement coroutine based on the input boolean
    /// Used when the OnMovementToggled Action is invoked
    /// </summary>
    /// <param name="change"> Determines if the movement should be turned on or off </param>
    private void ToggleMovement(bool change)
    {
        if (change)
        {
            _movementCoroutine = StartCoroutine("ResolveMovement");
        }
        else
        {
            StopCoroutine(_movementCoroutine);
            _rigidBody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Called when this component is enabled.
    /// Used to assign the OnMovementToggled Action to a listener
    /// </summary>
    private void OnEnable()
    {
        OnMovementToggled += ToggleMovement;
    }

    /// <summary>
    /// Called when this component is disnabled.
    /// Used to unassign the OnMovementToggled Action to a listener
    /// </summary>
    private void OnDisable()
    {
        OnMovementToggled -= ToggleMovement;
    }

    #region Getters

    #endregion

    #region Setters
    //Eventually I plan to change this by involving events, but for now we will use this
    public void SetFocusSpeedMultiplier(float newVal)
    {
        _currentFocusMoveSpeedMultiplier = _focusMoveSpeedCurve.Evaluate(newVal);
    }
    #endregion
}
