/*****************************************************************************
// File Name :         PlayerFunctionalityCore.cs
// Author :            Ryan Swanson, Alex Kalscheur
// Creation Date :     9/28/2024
//
// Brief Description : Holds higher level functionality to set up the player and harpoon
*****************************************************************************/

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
            _playerMovementController.SubscribeInput();
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
    }

    /// <summary>
    /// Unsubscribes to all needed events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        TimeManager.Instance.GetOnGamePauseEvent().RemoveListener(GamePaused);
        PlayerManager.Instance.GetOnPlayerDeath().RemoveListener(UnsubscribePlayerInput);
        TimeManager.Instance.GetOnGameUnpauseEvent().RemoveListener(GameUnpaused);
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
        _harpoonGun.SubscribeInput();
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
    }

    /// <summary>
    /// Unsubscribes from interaction input
    /// </summary>
    private void UnsubscribeToPlayerInteraction()
    {
        _playerInteraction.UnsubscribeInput();
    }
    #endregion

    #region Getters
    //TODO as needed
    public PlayerHealth GetPlayerHealth() => _playerHealthController;
    #endregion
}
