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

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovementController : MonoBehaviour
{
    /// <summary>
    /// Variables that relate to the horizontal movement of the player
    /// </summary>
    [Header("Adjustable Speed")]
    [SerializeField] private float _playerMovementSpeed;

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
    /// A variable to hold the Rigidbody
    /// </summary>
    private Rigidbody _rigidBody;

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
        // the rigibody
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
            * _playerMovementSpeed;

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
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -_cameraClamp, _cameraClamp);

        // Create a target rotation for the camera based on the player's rotation
        // and the new rotation we just found
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = _cameraXRotation;
        _playerCamera.eulerAngles = targetRotation;
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
