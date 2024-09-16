using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovementController : MonoBehaviour
{
    // IMPORTANT: THIS CLASS ALSO TEMPORARILY CONTROLS THE CAMERA
    // REMOVE THIS WHEN WE IMPLEMENT A MORE DYNAMIC CAMERA
    [SerializeField] private Transform _playerCamera;
    private float _cameraXRotation;

    // Horizontal Movement
    [SerializeField] private float _playerMovementSpeed;

    // Vertical Movement
    [SerializeField] private float _gravity;
    [SerializeField] private LayerMask _groundMask;
    private float _currentVerticalVelo;
    private bool _isGrounded;

    // Mouse Controls (ALSO USED FOR CAMERA)
    [SerializeField] private float _mouseSensitivityX;
    [SerializeField] private float _mouseSensitivityY;
    private float _cameraClamp;

    // Input
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    // THE BELOW INPUT ACTIONS ARE USED FOR THE CAMERA
    private InputAction _mouseXInput;
    private InputAction _mouseYInput;

    // Character Controller
    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize non-serialized variables, input variables, and
        // the character controller
        InitializeNonSerialized();
        InitializeInput();
        InitializeCharacter();

        // Run the movement coroutine
        StartCoroutine("StartMovement");
    }

    // Initializes all non-serialized private variables
    private void InitializeNonSerialized()
    {
        _cameraXRotation = 0;
        _currentVerticalVelo = 0;
        _isGrounded = true;
        _cameraClamp = 90;
    }

    // Initialize all input variables
    private void InitializeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();
        _movementInput = _playerInput.currentActionMap.FindAction("Movement");
        _mouseXInput = _playerInput.currentActionMap.FindAction("MouseX");
        _mouseYInput = _playerInput.currentActionMap.FindAction("MouseY");
    }

    // Get our Character Controller for movement
    private void InitializeCharacter()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Movement coroutine
    // This will perpetually call the movement handling methods until disabled
    private IEnumerator StartMovement()
    {
        while(true)
        {
            HandleHorizontalMovement();
            HandleVerticalMovement();
            HandleCameraRotation();
            yield return null;
        }
    }

    // Handler for horizontal movement based on input key presses
    private void HandleHorizontalMovement()
    {
        // Read the movement input
        Vector2 moveDir = _movementInput.ReadValue<Vector2>();

        // transform.right and transform.forward are vectors that point
        // in certain directions in the world
        // By manipulating them, we can move the character
        Vector3 newMovement = (transform.right * moveDir.x + transform.forward * moveDir.y) * _playerMovementSpeed;

        // Move the player
        _controller.Move(newMovement * Time.deltaTime);
    }

    // For now, handles gravity
    // This may be expanded to include jumping as well
    private void HandleVerticalMovement()
    {
        // Check to see if the player is on the ground
        _isGrounded = Physics.CheckSphere(transform.position, 0.1f, _groundMask);

        // If the player is on the ground, reset the downward velocity
        if (_isGrounded)
        {
            _currentVerticalVelo = 0;
        }

        // Make the player fall
        _currentVerticalVelo -= _gravity * Time.deltaTime;
        _controller.Move(new Vector3(0, _currentVerticalVelo, 0) * Time.deltaTime);
    }

    // THIS METHOD IS TEMPORARY UNTIL A CAMERA CONTROLLER IS CREATED
    // Handles the rotation of the camera based on mouse movement
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
}
