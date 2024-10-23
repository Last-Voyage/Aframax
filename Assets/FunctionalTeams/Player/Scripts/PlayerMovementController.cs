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
using UnityEngine.ProBuilder.MeshOperations;

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
    [Header("Adjustable Speed")]
    [SerializeField] private float _playerMovementSpeed;
    [Header("SlantedSurfaceLeniancy")]
    [SerializeField] private float _slantLeniancy;
    [Header("Adjustable Gravity")]
    [SerializeField] private float _gravity;

    private Transform _playerVisuals;
    private bool _isGrounded = false;
    private const float GROUNDED_CHECK_LENGTH = .2f;
    private Vector3 _groundedExtents = new Vector3(.05f, .05f, .05f);
    private Quaternion _groundNormal;
    RaycastHit groundHit;
    private Vector3 _slopeDirection;

    [Space]
    [SerializeField] private float _focusSpeedSlowTime;
    [SerializeField] private float _unfocusSpeedSlowTime;
    [SerializeField] private AnimationCurve _focusMoveSpeedCurve;

    private float _currentFocusMoveSpeedMultiplier = 1;

    private float _currentFocusMoveSpeedProgress = 0;

    private Coroutine _harpoonSlowdownCoroutine;

    [Space]
    [SerializeField] private LayerMask _walkableLayers;
    [SerializeField] private Transform _groundedCheckOrigin;

    /// <summary>
    /// Variables that capture user input
    /// </summary>
    private PlayerInput _playerInput;
    private InputAction _movementInput;
    private const string MOVEMENT_INPUT_NAME = "Movement";

    /// <summary>
    /// A variable to hold the Rigidbody
    /// </summary>
    private Rigidbody _rigidBody;

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
        SubscribeInput();
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
        _rigidBody = GetComponent<Rigidbody>();
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
        Vector3 horizontalMovement = HandleHorizontalMovement();
        Vector3 verticalMovement = HandleVerticalMovement();

        _rigidBody.velocity = horizontalMovement + verticalMovement;
    }

    private void GroundedCheck()
    {
        
        _isGrounded = Physics.BoxCast(_groundedCheckOrigin.position, _groundedExtents, 
            transform.up*-1, out groundHit, Quaternion.identity,GROUNDED_CHECK_LENGTH,_walkableLayers);


        if(_isGrounded)
        {
            /*_groundNormal = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
            float storedY = _playerVisuals.transform.eulerAngles.y;
            _playerVisuals.rotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
            _playerVisuals.transform.eulerAngles = 
                new Vector3(_playerVisuals.transform.eulerAngles.x, storedY, _playerVisuals.transform.eulerAngles.z);*/
            
        }
        else
        {
            //_groundNormal = Quaternion.identity;
        }

        

        print(_isGrounded);
        if (_isGrounded)
            print(groundHit.collider.gameObject.name);


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
        Vector3 newMovement = (_playerVisuals.right * moveDir.x +
            _playerVisuals.forward * moveDir.y) * 
            _playerMovementSpeed* _currentFocusMoveSpeedMultiplier;
        
        if(_isGrounded)
        {
            _slopeDirection = Vector3.ProjectOnPlane(newMovement, groundHit.normal).normalized;
        }
        else
        {
            _slopeDirection = newMovement;
        }

        Debug.DrawRay(transform.position, _slopeDirection, Color.blue, 1);
        newMovement = new Vector3(newMovement.x, 0, newMovement.z);

        print(_slopeDirection);
        //_slopeDirection = new Vector3(_slopeDirection.x, 0, _slopeDirection.z);
        return _slopeDirection *_playerMovementSpeed;
        // Move the player
        return newMovement;
    }

    private Vector3 HandleSlopeMovement()
    {
        return Vector3.zero;
    }

    /// <summary>
    /// Handles the force of gravity
    /// Can be expanded to include jumping later on
    /// </summary>
    private Vector3 HandleVerticalMovement()
    {
        if (_isGrounded)
        {
            return Vector3.zero;
        }

        Vector2 movement = _movementInput.ReadValue<Vector2>();

        print(_groundNormal);

        return new Vector3(0, _rigidBody.velocity.y, 0);
    }

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
            _rigidBody.velocity = Vector3.zero;
        }
    }
    #endregion

    private void OnCollisionStay(Collision collision)
    {
        
    }

    private void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}