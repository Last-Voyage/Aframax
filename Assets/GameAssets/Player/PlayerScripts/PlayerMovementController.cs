/******************************************************************************
// File Name:       PlayerMovementController.cs
// Author:          Andrew Stapay, Miles Rogers
// Creation Date:   September 15, 2024
//
// Description:     Implementation of the basic movement for a player character.
//                  This script takes input designated for movement from the
                    user and allows the player GameObject to move in the scene.
******************************************************************************/
using System.Collections;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

/// <summary>
/// A class that handless the movement of the player character.
/// Follow the steps outlined in the documentation to set this up in the editor.
/// When attached to the player, use the WASD keys to move around.
/// </summary>
public class PlayerMovementController : MonoBehaviour
{
    public static PlayerMovementController Instance;
    /// <summary>
    /// Variables that relate to the horizontal movement of the player
    /// </summary>
    [Header("Movement")]
    [Tooltip("The base movement speed of the player")]
    [SerializeField] private float _playerMovementSpeed;

    [Tooltip("The time it takes to reach max speed after initally pressing a directional input")]
    [SerializeField] private float _accelerationTime;
    [Tooltip("The time it takes to reset down the base speed")]
    [SerializeField] private float _deccelerationTime;
    [Tooltip("The curve showing how the player accelerates after pressing a directional input")]
    [SerializeField] private AnimationCurve _accelerationCurve;
    [Tooltip("The amount of gravity affecting the player (in m/s^2)")] 
    [SerializeField] private float _playerGravity = 9.8F;

    private float _currentAcceleration = 0;
    private float _accelerationProgress = 0;
    private Coroutine _accelerationCoroutine;
    private Coroutine _deccelerationCoroutine;
    private bool _isInputSubscribed;

    [Space]
    [Header("Focusing Movement")]
    [SerializeField] private float _focusSpeedSlowTime;
    [SerializeField] private float _unfocusSpeedSlowTime;
    [SerializeField] private AnimationCurve _focusMoveSpeedCurve;

    private float _currentFocusMoveSpeedMultiplier = 1;
    private float _currentFocusMoveSpeedProgress = 0;
    private Coroutine _harpoonSlowdownCoroutine;

    [Space]
    [Header("General")]
    [SerializeField] private float _groundedCheckLength = .2f;
    [SerializeField] private LayerMask _walkableLayers;

    private Transform _playerVisuals;
    public static bool IsGrounded { get; private set; } = false;
    private Transform _groundedCheckOrigin;
    public static bool IsMoving { get; private set; }

    [Tooltip("Size of boxcast for the grounded check")]
    private Vector3 _groundedExtents = new Vector3(.05f, .05f, .05f);
    RaycastHit _groundHit;

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    private const string _MOVEMENT_INPUT_NAME = "Movement";

    private Rigidbody _playerRigidBody;

    /// <summary>
    /// Movement coroutine related variables
    /// </summary>
    private Coroutine _movementCoroutine;

    #region Setup
    /// <summary>
    /// Used to initialized any variables
    /// Called by Functionality Core
    /// </summary>
    public void SetUpMovementController()
    {
        EstablishInstance();

        // Initialize input variables and the Rigidbody
        InitializeRigidbody();
        SetupPlayerVisuals();
        SetupPlayerGroundedCheckTransform();
    }

    /// <summary>
    /// Establishes the instance and removes
    /// </summary>
    private void EstablishInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Input Subscription
    /// <summary>
    /// Initializes all input variables
    /// </summary>
    public void SubscribeInput()
    {
        if (_isInputSubscribed)
        {
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();

        _movementInput = _playerInput.currentActionMap.FindAction(_MOVEMENT_INPUT_NAME);

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine(ResolveMovement());

        _isInputSubscribed = true;
    }

    /// <summary>
    /// Unsubscribes from all input
    /// </summary>
    public void UnsubscribeInput()
    {
        if (!_isInputSubscribed)
        {
            return;
        }
        _playerInput = null;
        StopCoroutine(_movementCoroutine);
        _isInputSubscribed = false;
    }
    #endregion

    #region Event Subscription
    /// <summary>
    /// Subscribes to all events not relating to input
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().AddListener(StartHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().AddListener(StopHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(StopHarpoonSpeedSlowdown);
    }

    /// <summary>
    /// Unsubscribes to all events not relating to input
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFocusStartEvent().RemoveListener(StartHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetOnHarpoonFocusEndEvent().RemoveListener(StopHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(StopHarpoonSpeedSlowdown);
    }
    #endregion
    
    /// <summary>
    /// Initializes our Rigidbody for gravity
    /// </summary>
    private void InitializeRigidbody()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Finds the object associated with player visuals
    /// </summary>
    private void SetupPlayerVisuals()
    {
        _playerVisuals = transform.GetChild(0);
    }

    /// <summary>
    /// Finds the object that the grounded check starts from
    /// </summary>
    private void SetupPlayerGroundedCheckTransform()
    {
        _groundedCheckOrigin = transform.GetChild(0).GetChild(0).transform;
    }

    /// <summary>
    /// Called when this component is enabled.
    /// Used to assign the OnMovementToggled Action to a listener
    /// </summary>
    private void OnEnable()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Called when this component is disabled.
    /// Used to unassign the OnMovementToggled Action to a listener
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeToEvents();
    }
    #endregion

    #region Movement
    /// <summary>
    /// Movement coroutine
    /// This will perpetually call the movement handling methods until disabled
    /// </summary>
    private IEnumerator ResolveMovement()
    {
        while (true)
        {
            DetermineInputState();
            GroundedCheck();
            HandleMovement();
            ApplyGravity();
            yield return null;
        }
    }

    /// <summary>
    /// Handles the general movement of the player
    /// Adds the horizontal and vertical movements to get the overall movement
    /// </summary>
    private void HandleMovement()
    {
        Vector3 horizontalMovement = DirectionalInputMovement();
        Vector3 verticalMovement = HandleVerticalMovement();

        _playerRigidBody.velocity = horizontalMovement + verticalMovement;
    }

    /// <summary>
    /// Determines if the player is grounded and takes the Raycasthit as an out variable
    /// </summary>
    private void GroundedCheck()
    {
        //Checks for if the player is grounded based on a boxcast
        IsGrounded = Physics.BoxCast(_groundedCheckOrigin.position, _groundedExtents, 
            transform.up*-1, out _groundHit, Quaternion.identity,_groundedCheckLength,_walkableLayers);
    }

    /// <summary>
    /// Handler for movement based on input key presses
    /// </summary>
    private Vector3 DirectionalInputMovement()
    {
        // Read the movement input
        Vector2 moveDir = _movementInput.ReadValue<Vector2>();

        // transform.right and transform.forward are vectors that point
        // in certain directions in the world
        // By manipulating them, we can move the character
        Vector3 newMovement = (_playerVisuals.right * moveDir.x +
            _playerVisuals.forward * moveDir.y);
        
        if(IsGrounded)
        {
            //Projects the movement onto the surface that is being stood on
            //This movement will be vertical when on a sloped surface
            newMovement = Vector3.ProjectOnPlane(newMovement, _groundHit.normal).normalized;
        }
        
        // Returns the movement direction times the speed and acceleration
        return newMovement * (_playerMovementSpeed * _currentFocusMoveSpeedMultiplier * _currentAcceleration);
    }
    
    /// <summary>
    /// Handles any needed vertical force
    /// </summary>
    private Vector3 HandleVerticalMovement()
    {
        if (IsGrounded)
        {
            return Vector3.zero;
        }

        return new Vector3(0, _playerRigidBody.velocity.y, 0);
    }

    #region Acceleration
    /// <summary>
    /// Determines if the movement was started or stopped this frame
    /// </summary>
    private void DetermineInputState()
    {
        //Check for if the input was started
        if(_movementInput.WasPressedThisFrame())
        {
            DirectionalInputStarted(_movementInput);
            IsMoving = true;
        }
        //Check for if the input has ended
        else if (_movementInput.WasReleasedThisFrame())
        {
            DirectionalInputStopped();
            IsMoving = false;
        }
    }

    /// <summary>
    /// Called when the player begins an input
    /// </summary>
    private void DirectionalInputStarted(InputAction playerMovement)
    {
        PlayerManager.Instance.OnInvokeMovementStartedEvent(playerMovement);

        StopAccelerationDeccelerationCoroutines();

        StartInitialMovementAcceleration();
    }

    /// <summary>
    /// Called when the player ends an input
    /// </summary>
    private void DirectionalInputStopped()
    {
        PlayerManager.Instance.OnInvokeMovementEndedEvent();

        StopAccelerationDeccelerationCoroutines();

        StartMovementDecceleration();
    }

    /// <summary>
    /// Starts the acceleration for when the player begins walking
    /// </summary>
    private void StartInitialMovementAcceleration()
    {
        _accelerationCoroutine = StartCoroutine(InitialMovementAcceleration());
    }

    /// <summary>
    /// The process of accelerating
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitialMovementAcceleration()
    {
        //While for when the acceleration isn't at max
        while(_accelerationProgress < 1)
        {
            //Increase the acceleration progress by time divided by the time it takes to accelerate
            _accelerationProgress += Time.deltaTime / _accelerationTime;
            //Calculates the current acceleration
            EvaluateCurrentAcceleration();
            yield return null;
        }
        //Makes certain the acceleration is set to 1 and wasn't overshot
        _accelerationProgress = 1;
        EvaluateCurrentAcceleration();
    }

    /// <summary>
    /// Starts the decceleration
    /// </summary>
    private void StartMovementDecceleration()
    {
        _deccelerationCoroutine = StartCoroutine(MovementDecceleration());
    }

    /// <summary>
    /// Resets the acceleration after staying stationary for a short time
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementDecceleration()
    {
        //Wait for a short period before reseting the acceleration
        yield return new WaitForSeconds(_deccelerationTime);
        _accelerationProgress = 0;
        EvaluateCurrentAcceleration();
    }

    /// <summary>
    /// Calculates the current acceleration based on the acceleration progress
    /// </summary>
    private void EvaluateCurrentAcceleration()
    {
        _currentAcceleration = _accelerationCurve.Evaluate(_accelerationProgress);
    }

    /// <summary>
    /// Stops both the acceleration and decceleration
    /// </summary>
    private void StopAccelerationDeccelerationCoroutines()
    {
        if (_accelerationCoroutine != null)
        {
            StopCoroutine(_accelerationCoroutine);
        }
        if(_deccelerationCoroutine != null)
        {
            StopCoroutine(_deccelerationCoroutine);
        }
    }
    #endregion
    
    #region Gravity

    /// <summary>
    /// Add a downward velocity vector to the player's Rigidbody
    /// </summary>
    private void ApplyGravity()
    {
        // If no input and on the ground, don't allow the Rigidbody to move
        if (!IsMoving && IsGrounded)
        {
            _playerRigidBody.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        
        // Ensure the Rigidbody can move
        _playerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        
        // Apply the downward gravity force
        _playerRigidBody.AddForce(
            new Vector3(0.0F, -_playerGravity * Time.deltaTime, 0.0F)
        );
    }
    
    #endregion

    #region Harpoon Slowdown
    /// <summary>
    /// Starts the harpoon speed slowdown
    /// </summary>
    private void StartHarpoonSpeedSlowdown()
    {
        StopCurrentFocusCoroutine();
        _harpoonSlowdownCoroutine = StartCoroutine(HarpoonSpeedSlowdownProcess());
    }

    /// <summary>
    /// Stops the slowdown from focusing the harpoon
    /// </summary>
    private void StopHarpoonSpeedSlowdown()
    {
        StopCurrentFocusCoroutine();
        _harpoonSlowdownCoroutine = StartCoroutine(HarpoonSpeedUpProcess());
    }

    /// <summary>
    /// Stops the process of focusing or unfocusing
    /// </summary>
    private void StopCurrentFocusCoroutine()
    {
        if (_harpoonSlowdownCoroutine != null)
        {
            StopCoroutine(_harpoonSlowdownCoroutine);
        }
    }

    /// <summary>
    /// The process of slowing down the player while focusing
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarpoonSpeedSlowdownProcess()
    {
        while (_currentFocusMoveSpeedProgress < 1)
        {
            //Increases the progress on slowdown
            _currentFocusMoveSpeedProgress += Time.deltaTime / _focusSpeedSlowTime;

            CalculateCurrentFocusSpeedMultiplier();
            yield return null;
        }
    }

    /// <summary>
    /// The process of speeding up the player after unfocusing
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarpoonSpeedUpProcess()
    {
        while (_currentFocusMoveSpeedProgress > 0)
        {
            //Decreases the progress on slowdown
            _currentFocusMoveSpeedProgress -= Time.deltaTime / _unfocusSpeedSlowTime;

            CalculateCurrentFocusSpeedMultiplier();

            yield return null;
        }

        HarpoonSpeedUpComplete();
    }

    /// <summary>
    /// Called when the player is at max speed after unfocusing their weapon
    /// </summary>
    private void HarpoonSpeedUpComplete()
    {
        _currentFocusMoveSpeedProgress = 0;
        CalculateCurrentFocusSpeedMultiplier();
    }

    /// <summary>
    /// Calculates the current speed multiplier for focusing the weapon
    /// </summary>
    private void CalculateCurrentFocusSpeedMultiplier()
    {
        _currentFocusMoveSpeedMultiplier = _focusMoveSpeedCurve.Evaluate(_currentFocusMoveSpeedProgress);
    }

    #endregion

    #endregion

    #region  Getters

    #endregion Getters
}