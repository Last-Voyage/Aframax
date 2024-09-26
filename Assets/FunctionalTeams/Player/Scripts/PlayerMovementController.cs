/******************************************************************************
// File Name:       PlayerMovementController.cs
// Author:          Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Implementation of the basic movement for a player character.
//                  This script takes input designated for movement from the
                    user and allows the player GameObject to move in the scene.
******************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    private const string _MOVEMENT_INPUT_NAME = "Movement";

    /// <summary>
    /// A variable to hold the Rigidbody
    /// </summary>
    private Rigidbody _rigidBody;

    /// <summary>
    /// Movement coroutine related variables
    /// </summary>
    private static UnityEvent<bool> OnMovementToggled;
    private Coroutine _movementCoroutine;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized.
    /// </summary>
    void Start()
    {
        // Initialize input variables and the Rigidbody
        InitializeInput();
        InitializeRigidbody();

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine(ResolveMovement());
    }

    /// <summary>
    /// Initializes all input variables
    /// </summary>
    private void InitializeInput()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();
        _movementInput = _playerInput.currentActionMap.FindAction(_MOVEMENT_INPUT_NAME);
    }

    /// <summary>
    /// Initializes our Rigidbody for gravity
    /// </summary>
    private void InitializeRigidbody()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Movement coroutine
    /// This will perpetually call the movement handling methods until disabled
    /// </summary>
    private IEnumerator ResolveMovement()
    {
        while (true)
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
        Vector3 newMovement = (transform.right * moveDir.x + transform.forward * moveDir.y) * _playerMovementSpeed;

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
    /// Activates or deactivates the movement coroutine based on the input boolean
    /// Used when the OnMovementToggled Action is invoked
    /// </summary>
    /// <param name="change"> Determines if the movement should be turned on or off </param>
    private void ToggleMovement(bool change)
    {
        if (change)
        {
            _movementCoroutine = StartCoroutine(ResolveMovement());
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
        OnMovementToggled.AddListener(ToggleMovement);
    }

    /// <summary>
    /// Called when this component is disabled.
    /// Used to unassign the OnMovementToggled Action to a listener
    /// </summary>
    private void OnDisable()
    {
        OnMovementToggled.RemoveListener(ToggleMovement);
    }

    #region Getters
    /// <summary>
    /// Getter for the OnMovementToggle event
    /// </summary>
    public UnityEvent<bool> GetMovementToggle => OnMovementToggled;
    #endregion
}