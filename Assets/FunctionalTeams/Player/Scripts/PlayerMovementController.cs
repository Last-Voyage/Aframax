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
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
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
    [SerializeField] private float _playerMovementSpeed;

    [SerializeField] private float _accelerationTime;
    [SerializeField] private float _deccelerationTime;
    [SerializeField] private AnimationCurve _accelerationCurve;

    private float _currentAcceleration = 0;
    private float _accelerationProgress = 0;
    private Coroutine _accelerationCoroutine;
    private Coroutine _deccelerationCoroutine;

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
    [SerializeField] private LayerMask _walkableLayers;
    [SerializeField] private Transform _groundedCheckOrigin;

    private Transform _playerVisuals;
    private bool _isGrounded = false;
    private const float GROUNDED_CHECK_LENGTH = .2f;
    [Tooltip("Size of boxcast for the grounded check")]
    private Vector3 _groundedExtents = new Vector3(.05f, .05f, .05f);
    RaycastHit _groundHit;

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    private const string MOVEMENT_INPUT_NAME = "Movement";

    private Rigidbody _playerRigidBody;

    /// <summary>
    /// Movement coroutine related variables
    /// </summary>
    private Coroutine _movementCoroutine;

    #region Setup
    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized.
    /// </summary>
    private void Start()
    {
        EstablishInstance();

        // Initialize input variables and the Rigidbody
        SubscribeToEvents();
        InitializeRigidbody();
        SetupPlayerVisuals();

        // Run the movement coroutine
        _movementCoroutine = StartCoroutine(ResolveMovement());
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
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.currentActionMap.Enable();

        _movementInput = _playerInput.currentActionMap.FindAction(MOVEMENT_INPUT_NAME);
    }

    /// <summary>
    /// Unsubscribes from all input
    /// </summary>
    public void UnsubscribeInput()
    {
        _movementInput = null;
    }
    #endregion

    #region Event Subscription
    /// <summary>
    /// Subscribes to all events not relating to input
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetMovementToggleEvent().AddListener(ToggleMovement);
        PlayerManager.Instance.GetHarpoonFocusStartEvent().AddListener(StartHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetHarpoonFocusEndEvent().AddListener(StopHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetHarpoonFiredStartEvent().AddListener(StopHarpoonSpeedSlowdown);
    }

    /// <summary>
    /// Unsubscribes to all events not relating to input
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetMovementToggleEvent().RemoveListener(ToggleMovement);
        PlayerManager.Instance.GetHarpoonFocusStartEvent().RemoveListener(StartHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetHarpoonFocusEndEvent().RemoveListener(StopHarpoonSpeedSlowdown);
        PlayerManager.Instance.GetHarpoonFiredStartEvent().RemoveListener(StopHarpoonSpeedSlowdown);
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
        _isGrounded = Physics.BoxCast(_groundedCheckOrigin.position, _groundedExtents, 
            transform.up*-1, out _groundHit, Quaternion.identity,GROUNDED_CHECK_LENGTH,_walkableLayers);

        _playerRigidBody.useGravity = !_isGrounded;
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
            _playerVisuals.forward * moveDir.y) * _currentFocusMoveSpeedMultiplier;
        
        if(_isGrounded)
        {
            //Projects the movement onto the surface that is being stood on
            //This movement will be vertical when on a sloped surface
            newMovement = Vector3.ProjectOnPlane(newMovement, _groundHit.normal).normalized;
        }
        
        // Returns the movement direction times the speed and acceleration
        return newMovement * _playerMovementSpeed * _currentAcceleration;
    }

    /// <summary>
    /// Handles any needed vertical force
    /// </summary>
    private Vector3 HandleVerticalMovement()
    {
        if (_isGrounded)
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
        if(_movementInput.WasPressedThisFrame())
        {
            DirectionalInputStarted();
        }
        else if (_movementInput.WasReleasedThisFrame())
        {
            DirectionalInputStopped();
        }
    }

    /// <summary>
    /// Called when the player begins an input
    /// </summary>
    private void DirectionalInputStarted()
    {
        StopAccelerationDeccelerationCoroutines();

        StartInitialMovementAcceleration();
    }

    /// <summary>
    /// Called when the player ends an input
    /// </summary>
    private void DirectionalInputStopped()
    {
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
        while(_accelerationProgress < 1)
        {
            _accelerationProgress += Time.deltaTime / _accelerationTime;
            EvaluateCurrentAcceleration();
            yield return null;
        }
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
            _playerRigidBody.velocity = Vector3.zero;
        }
    }
    #endregion
}