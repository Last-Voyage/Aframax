/******************************************************************************
// File Name:       PlayerMovementController.cs
// Author:          Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Implementation of the basic movement for a player character.
//                  This script takes input designated for movement from the
//                  user and allows the player GameObject to move in the scene.
******************************************************************************/
using System.Collections;
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

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;

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
    /// Used to initialize any variables that are not serialized
    /// and to start the coroutine
    /// </summary>
    void Start()
    {
        // Initialize input variables and the Rigidbody
        InitializeInput();
        InitializeRigidbody();

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine("ResolveMovement");
    }

    /// <summary>
    /// Initializes all input variables
    /// </summary>
    private void InitializeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();
        _movementInput = _playerInput.currentActionMap.FindAction("Movement");
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
    /// This will perpetually call the movement handling method until disabled
    /// </summary>
    private IEnumerator ResolveMovement()
    {
        while(true)
        {
            HandleMovement();
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
