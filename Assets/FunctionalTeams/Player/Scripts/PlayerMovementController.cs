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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovementController : MonoBehaviour
{
    /// <summary>
    /// Variables that relate to the horizontal movement of the player
    /// </summary>
    [Header("Adjustable Speed")]
    [SerializeField] private float _playerMovementSpeed;

    /// <summary>
    /// Variables that relate to the vertical movement of the player
    /// </summary>
    [Header("Adjustable Gravity")]
    [SerializeField] private float _gravity;
    [Header("Set Layer Mask for Ground Here")]
    [SerializeField] private LayerMask _groundMask;
    private const float _GROUND_CHECK_RADIUS = 0.1f;
    private float _currentVerticalVelo;
    private bool _isGrounded;

    // IMPORTANT: THIS CLASS ALSO TEMPORARILY CONTROLS THE CAMERA
    // REMOVE THIS WHEN WE IMPLEMENT A MORE DYNAMIC CAMERA
    /// <summary>
    /// Variables that relate to the rotation of the camera
    /// </summary>
    [Header("Set Player Camera Here")]
    [SerializeField] private Transform _playerCamera;
    private float _cameraXRotation;
    [Header("Adjustable Mouse Sensitivities")]
    [Tooltip("X controls Left/Right\nY controls Up/Down")]
    [SerializeField] private float _mouseSensitivityX;
    [SerializeField] private float _mouseSensitivityY;
    private float _cameraClamp;

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    // THE BELOW INPUT ACTIONS ARE USED FOR THE CAMERA
    private InputAction _mouseXInput;
    private InputAction _mouseYInput;

    /// <summary>
    /// A variable to hold the Character Controller
    /// </summary>
    private CharacterController _characterController;

    /// <summary>
    /// Coroutine variable to hold our movement coroutine
    /// </summary>
    private Coroutine _movementCoroutine;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized.
    /// </summary>
    void Start()
    {
        // Initialize non-serialized variables, input variables, and
        // the character controller
        InitializeNonSerialized();
        InitializeInput();
        InitializeCharacter();

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine("ResolveMovement");
    }

    /// <summary>
    /// Initializes all non-serialized private variables.
    /// </summary>
    private void InitializeNonSerialized()
    {
        _cameraXRotation = 0;
        _currentVerticalVelo = 0;
        _isGrounded = true;
        _cameraClamp = 90;
    }

    /// <summary>
    /// Initializes all input variables
    /// </summary>
    private void InitializeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();
        _movementInput = _playerInput.currentActionMap.FindAction("Movement");
        _mouseXInput = _playerInput.currentActionMap.FindAction("MouseX");
        _mouseYInput = _playerInput.currentActionMap.FindAction("MouseY");
    }

    /// <summary>
    /// Initializes our Character Controller for movement
    /// </summary>
    private void InitializeCharacter()
    {
        _characterController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Movement coroutine
    /// This will perpetually call the movement handling methods until disabled
    /// </summary>
    private IEnumerator ResolveMovement()
    {
        while(true)
        {
            HandleHorizontalMovement();
            HandleVerticalMovement();
            HandleCameraRotation();
            yield return null;
        }
    }

    /// <summary>
    /// Handler for horizontal movement based on input key presses
    /// </summary>
    private void HandleHorizontalMovement()
    {
        // Read the movement input
        Vector2 moveDir = _movementInput.ReadValue<Vector2>();

        // transform.right and transform.forward are vectors that point
        // in certain directions in the world
        // By manipulating them, we can move the character
        Vector3 newMovement = (transform.right * moveDir.x + transform.forward * moveDir.y)
            * _playerMovementSpeed;

        // Move the player
        _characterController.Move(newMovement * Time.deltaTime);
    }

    /// <summary>
    /// Handles the force of gravity
    /// Can be expanded to include jumping later on
    /// </summary>
    private void HandleVerticalMovement()
    {
        // Check to see if the player is on the ground
        _isGrounded = CheckGrounded();

        // If the player is on the ground, reset the downward velocity
        if (_isGrounded)
        {
            _currentVerticalVelo = 0;
        }

        // Make the player fall
        _currentVerticalVelo -= _gravity * Time.deltaTime;
        _characterController.Move(new Vector3(0, _currentVerticalVelo, 0) * Time.deltaTime);
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
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -_cameraClamp, _cameraClamp);

        // Create a target rotation for the camera based on the player's rotation
        // and the new rotation we just found
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = _cameraXRotation;
        _playerCamera.eulerAngles = targetRotation;
    }

    /// <summary>
    /// Checks to see if the player is on the ground
    /// </summary>
    /// <returns>
    /// True if the player is on the ground or false otherwise
    /// </returns>
    private bool CheckGrounded()
    {
        return Physics.CheckSphere(transform.position, _GROUND_CHECK_RADIUS, _groundMask);
    }

    /// <summary>
    /// Temporarily stops the movement coroutine
    /// </summary>
    public void PauseMovement()
    {
        StopCoroutine(_movementCoroutine);
    }

    /// <summary>
    /// Restarts the movement coroutine after pausing it
    /// </summary>
    public void RestartMovement()
    {
        _movementCoroutine = StartCoroutine("ResolveMovement");
    }
}
