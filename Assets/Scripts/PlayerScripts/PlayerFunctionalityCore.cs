/*****************************************************************************
// File Name :         PlayerFunctionalityCore.cs
// Author :            Ryan Swanson, Alex Kalscheur
// Creation Date :     9/28/2024
//
// Brief Description : Holds higher level functionality to set up the player and harpoon
*****************************************************************************/

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Holds functionality for managing the relationship of player scripts 
/// and centralizing certain functionality
/// </summary>
public class PlayerFunctionalityCore : MonoBehaviour
{
    //Contains all the associated player functionality
    //Controls player movement functionality
    [SerializeField] private PlayerMovementController _playerMovementController;
    //Controls player health functionality
    [SerializeField] private PlayerHealth _playerHealthController;
    //Controls camera movement
    public PlayerCameraController PlayerCamera;
    //Controls harpoon weapon functionality
    [SerializeField] private HarpoonGun _harpoonGun;
    //Controls player interaction functionality
    [SerializeField] private PlayerInteraction _playerInteraction;

    private PlayerInputMap _playerInputMap;

    private bool _subscribedToInput;

    public static PlayerFunctionalityCore Instance;

    private Coroutine _cameraReturningCoroutine = null;
    private Vector3 _cameraPositionWithLastInput = Vector3.zero;

    /// <summary>
    /// Performs any set up before everything else
    /// </summary>
    private void Awake()
    {
        SetUpPlayer();
    }

    /// <summary>
    /// Performs any set up in the player
    /// This occurs before any player input subscription
    /// Used for if any player functionality needs to called before movement begins
    /// </summary>
    private void SetUpPlayer()
    {
        Instance = this;
        // Sets needed variables in the player movement controller before movement begins
        _playerMovementController.SetUpMovementController();
        _playerHealthController.SetUpHealth();
        PlayerCamera.CameraSetup();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
        SubscribePlayerInput();
    }

    private void OnDisable()
    {
        UnsubscribeToEvents();
        UnsubscribePlayerInput();
    }

    #region Input
    /// <summary>
    /// Subscribes to all needed input
    /// </summary>
    private void SubscribePlayerInput()
    {
        if (_subscribedToInput) { return; }

        _playerInputMap = new();
        _playerInputMap.Enable();

        SubscribeToMovementInput();
        SubscribeToCameraInput();
        SubscribeToHarpoonInput();
        SubscribeToPlayerInteraction();

        PlayerManager.Instance.OnInvokePlayerInputToggle(true);

        _subscribedToInput = true;
    }

    /// <summary>
    /// Subscribes to movement input
    /// </summary>
    private void SubscribeToMovementInput()
    {
        if(PlayerSpawnPoint.Instance.CanSpawnWithMovement)
        {
            if (Camera.main.transform.localPosition == _cameraPositionWithLastInput)
            {
                _playerMovementController.SubscribeInput();
            }
            else
            {
                DelayInputCinmatics();
            }
        }
    }

    /// <summary>
    /// Subscribes to camera input
    /// </summary>
    private void SubscribeToCameraInput()
    {
        PlayerCamera.SubscribeInput();
    }

    /// <summary>
    /// Subscribes to all needed events
    /// </summary>
    private void SubscribeToEvents()
    {
        TimeManager.Instance.GetOnGamePauseEvent().AddListener(GamePaused);
        PlayerManager.Instance.GetOnPlayerDeath().AddListener(UnsubscribePlayerInput);
        TimeManager.Instance.GetOnGameUnpauseEvent().AddListener(GameUnpaused);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(UnsubscribeToMovementInput);
        CameraManager.Instance.GetOnCinematicStartEvent().AddListener(UnsubscribeToHarpoonInput);
        CameraManager.Instance.GetOnCinematicEndEvent().AddListener(SubscribeToMovementInput);
        CameraManager.Instance.GetOnCinematicEndEvent().AddListener(SubscribeToHarpoonInput);
    }

    /// <summary>
    /// Unsubscribes to all needed events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        TimeManager.Instance.GetOnGamePauseEvent().RemoveListener(GamePaused);
        PlayerManager.Instance.GetOnPlayerDeath().RemoveListener(UnsubscribePlayerInput);
        TimeManager.Instance.GetOnGameUnpauseEvent().RemoveListener(GameUnpaused);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(UnsubscribeToMovementInput);
        CameraManager.Instance.GetOnCinematicStartEvent().RemoveListener(UnsubscribeToHarpoonInput);
        CameraManager.Instance.GetOnCinematicEndEvent().RemoveListener(SubscribeToMovementInput);
        CameraManager.Instance.GetOnCinematicEndEvent().RemoveListener(SubscribeToHarpoonInput);
    }

    /// <summary>
    /// Unsubscribes player input when the game is paused
    /// </summary>
    private void GamePaused()
    {
        UnsubscribePlayerInput();
    }

    /// <summary>
    /// Subscribes player input when the game is unpaused
    /// </summary>
    private void GameUnpaused()
    {
        SubscribePlayerInput();
    }    

    /// <summary>
    /// Subscribes to harpoon input
    /// </summary>
    private void SubscribeToHarpoonInput()
    {
        if (Camera.main.transform.localPosition == _cameraPositionWithLastInput)
        {
            _harpoonGun.SubscribeInput();
        }
        else
        {
            DelayInputCinmatics();
        }
    }

    /// <summary>
    /// Subscribes to interaction input
    /// </summary>
    private void SubscribeToPlayerInteraction()
    {
        _playerInteraction.SubscribeInput();
    }

    /// <summary>
    /// Unsubscribes to all input
    /// </summary>
    private void UnsubscribePlayerInput()
    {
        if (!_subscribedToInput) { return; }
        
        UnsubscribeToMovementInput();
        UnsubscribeToCameraInput();
        UnsubscribeToHarpoonInput();
        UnsubscribeToPlayerInteraction();

        _playerInputMap.Disable();

        PlayerManager.Instance.OnInvokePlayerInputToggle(false);

        _subscribedToInput = false;
    }

    /// <summary>
    /// Unsubscribes to movement input
    /// </summary>
    private void UnsubscribeToMovementInput()
    {
        _playerMovementController.UnsubscribeInput();
        _cameraPositionWithLastInput = Camera.main.transform.localPosition;
    }

    /// <summary>
    /// Unsubscribes to camera input
    /// </summary>
    private void UnsubscribeToCameraInput()
    {
        PlayerCamera.UnsubscribeInput();
    }

    /// <summary>
    /// Unsubscribes to harpoon input
    /// </summary>
    private void UnsubscribeToHarpoonInput()
    {
        _harpoonGun.UnsubscribeInput();
        _cameraPositionWithLastInput = Camera.main.transform.localPosition;
    }

    /// <summary>
    /// Unsubscribes from interaction input
    /// </summary>
    private void UnsubscribeToPlayerInteraction()
    {
        _playerInteraction.UnsubscribeInput();
    }

    /// <summary>
    /// Delays the return of player input after a cinematic has played.
    /// </summary>
    private void DelayInputCinmatics()
    {
        if (_cameraReturningCoroutine.IsUnityNull())
        {
            _cameraReturningCoroutine = StartCoroutine(SubscribeInputWithDelay());
        }
    }

    /// <summary>
    /// Variation of SubcribeInput with a delay for the camera to return to its original location
    /// Intended to be used after cinematics
    /// </summary>
    private IEnumerator SubscribeInputWithDelay()
    {
        yield return new WaitUntil(WaitForCameraReturn);

        _playerMovementController.SubscribeInput();
        _harpoonGun.SubscribeInput();

        _cameraReturningCoroutine = null;
    }

    /// <summary>
    /// Bool to check if the camera has returned to its original position after cinematics
    /// </summary>
    /// <returns> True if the camera is in the correct position, false otherwise </returns>
    private bool WaitForCameraReturn()
    {
        return Camera.main.transform.localPosition == _cameraPositionWithLastInput;
    }
    #endregion

    #region Getters
    //TODO as needed
    public PlayerHealth GetPlayerHealth() => _playerHealthController;
    #endregion
}
