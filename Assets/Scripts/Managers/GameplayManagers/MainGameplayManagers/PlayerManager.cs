/******************************************************************************
// File Name:       PlayerManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Provides other all other scripts with access to the player
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Numerics;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Provides all other scripts with access to the player
/// </summary>
public class PlayerManager : MainGameplayManagerFramework
{
    public PlayerSpawnPoint _spawnPoint;

    public static PlayerManager Instance;

    /// <summary>
    /// Controls the player's movement
    /// </summary>
    private static readonly UnityEvent<bool> _onPlayerInputToggled = new();

    //When the movement starts
    private static readonly UnityEvent<InputAction> _onMovementStartedEvent = new();
    //When the movement stops
    private static readonly UnityEvent _onMovementEndedEvent = new();

    //When the projectile is fired
    private static readonly UnityEvent _onHarpoonFiredEvent = new();
    //When the harpoon gun starts reloading
    private static readonly UnityEvent _onHarpoonReloadStartEvent = new();
    //When the harpoon is reloaded
    private static readonly UnityEvent _onHarpoonReloadedEvent = new();
    //When the harpoon's ammo is restocked
    private static readonly UnityEvent<AmmoRackInteractable> _onHarpoonRestockEvent = new();
    //When the harpoon's ammo restocking is complete
    private static readonly UnityEvent<int> _onHarpoonRestockCompleteEvent = new();

    //When the player starts focusing
    private static readonly UnityEvent _onHarpoonFocusStartEvent = new();
    //When the player is at max focus
    private static readonly UnityEvent _onHarpoonFocusMaxEvent = new();
    //When the player is no longer focused
    private static readonly UnityEvent _onHarpoonFocusEndEvent = new();

    private static readonly UnityEvent _onEnemyOverCrosshairStartEvent = new();
    private static readonly UnityEvent _onEnemyOverCrosshairEndEvent = new();

    //When the player takes damage
    private static readonly UnityEvent<float> _onPlayerDamageEvent = new();
    //When the player receives healing
    private static readonly UnityEvent<float> _onPlayerHealEvent = new();
    //When the player health changes for any reason
    private static readonly UnityEvent<float, float> _onHealthChange = new();
    //When the player dies
    private static readonly UnityEvent _onPlayerDeath = new();

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the player manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Overrides the base class by also setting up the spawn point
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        _spawnPoint.SetUp();
    }

    #endregion

    #region Events
    /// <summary>
    /// Invokes the _onMovementToggled event with the input bool
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    public void OnInvokePlayerInputToggle(bool toggle)
    {
        _onPlayerInputToggled?.Invoke(toggle);
    }

    /// <summary>
    /// Invokes when the player movement starts
    /// </summary>
    /// <param name="playerMovement"> The InputAction associated with the player's movement </param>
    public void OnInvokeMovementStartedEvent(InputAction playerMovement)
    {
        _onMovementStartedEvent?.Invoke(playerMovement);
    }

    /// <summary>
    /// Invokes when the player movement ends
    /// </summary>
    public void OnInvokeMovementEndedEvent()
    {
        _onMovementEndedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the harpoon projectile is fired
    /// </summary>
    public void OnInvokeHarpoonFiredEvent()
    {
        _onHarpoonFiredEvent?.Invoke();
    }

    /// <summary>
    /// IInvokes event for when the harpoon starts reloading
    /// </summary>
    public void OnInvokeHarpoonStartReloadEvent()
    {
        _onHarpoonReloadStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the harpoon is reloaded
    /// </summary>
    public void OnInvokeHarpoonReloadedEvent()
    {
        _onHarpoonReloadedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the harpoon restocks its ammo
    /// </summary>
    /// <param name="ammoRack"> The ammo rack where the ammo is taken from </param>
    public void OnInvokeHarpoonRestockEvent(AmmoRackInteractable ammoRack)
    {
        _onHarpoonRestockEvent?.Invoke(ammoRack);
    }

    /// <summary>
    /// Invokes event for when the harpoon is finished restocking
    /// </summary>
    /// <param name="numHarpoons"> the number of harpoons that were restocked </param>
    public void OnInvokeHarpoonRestockCompleteEvent(int numHarpoons)
    {
        _onHarpoonRestockCompleteEvent?.Invoke(numHarpoons);
    }

    /// <summary>
    /// Invokes the harpoon focus start event
    /// </summary>
    public void OnInvokeHarpoonFocusStartEvent()
    {
        _onHarpoonFocusStartEvent?.Invoke();
    }
    
    /// <summary>
    /// Invokes the harpoon focus max event
    /// </summary>
    public void OnInvokeHarpoonFocusMaxEvent()
    {
        _onHarpoonFocusMaxEvent?.Invoke();
    }
    
    /// <summary>
    /// Invokes the harpoon focus end event
    /// </summary>
    public void OnInvokeHarpoonFocusEndEvent()
    {
        _onHarpoonFocusEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy start event
    /// </summary>
    public void OnInvokeCrosshairOverEnemyStartEvent()
    {
        _onEnemyOverCrosshairStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy end event
    /// </summary>
    public void OnInvokeCrosshairOverEnemyEndEvent()
    {
        _onEnemyOverCrosshairEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes when the player receives damage
    /// </summary>
    /// <param name="damageTaken">The damage received</param>
    public void OnInvokePlayerDamagedEvent(float damageTaken)
    {
        _onPlayerDamageEvent?.Invoke(damageTaken);
    }

    /// <summary>
    /// Invokes when the player receives healing
    /// </summary>
    /// <param name="healTaken">The healing received</param>
    public void OnInvokePlayerHealEvent(float healTaken)
    {
        _onPlayerHealEvent?.Invoke(healTaken);
    }

    /// <summary>
    /// Invokes when the player health changes by any means
    /// </summary>
    /// <param name="percentHealth">The current percent health</param>
    /// <param name="currentHealth">The current health amount</param>
    public void OnInvokePlayerHealthChangeEvent(float percentHealth, float currentHealth)
    {
        _onHealthChange?.Invoke(percentHealth, currentHealth);
    }

    /// <summary>
    /// Invokes when the player dies
    /// </summary>
    public void OnInvokePlayerDeath()
    {
        _onPlayerDeath?.Invoke();
    }
    #endregion

    #region Getters
    
    /// <summary>
    /// Getter for the _onMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetOnInputToggleEvent() => _onPlayerInputToggled;

    public UnityEvent<InputAction> GetOnMovementStartEvent() => _onMovementStartedEvent;
    public UnityEvent GetOnMovementEndEvent() => _onMovementEndedEvent;

    public UnityEvent GetOnHarpoonFiredEvent() => _onHarpoonFiredEvent;
    public UnityEvent GetOnHarpoonStartReloadEvent() => _onHarpoonReloadStartEvent;
    public UnityEvent GetOnHarpoonReloadedEvent() => _onHarpoonReloadedEvent;
    public UnityEvent<AmmoRackInteractable> GetOnHarpoonRestockEvent() => _onHarpoonRestockEvent;
    public UnityEvent<int> GetOnHarpoonRestockCompleteEvent() => _onHarpoonRestockCompleteEvent;

    public UnityEvent GetOnHarpoonFocusStartEvent() => _onHarpoonFocusStartEvent;
    public UnityEvent GetOnHarpoonFocusMaxEvent() => _onHarpoonFocusMaxEvent;
    public UnityEvent GetOnHarpoonFocusEndEvent() => _onHarpoonFocusEndEvent;

    public UnityEvent GetOnEnemyOverCrosshairStartEvent() => _onEnemyOverCrosshairStartEvent;
    public UnityEvent GetOnEnemyOverCrosshairEndEvent() => _onEnemyOverCrosshairEndEvent;

    public UnityEvent<float> GetOnPlayerDamageEvent() => _onPlayerDamageEvent;
    public UnityEvent<float> GetOnPlayerHealEvent() => _onPlayerHealEvent;
    public UnityEvent<float, float> GetOnPlayerHealthChangeEvent() => _onHealthChange;
    public UnityEvent GetOnPlayerDeath() => _onPlayerDeath;
    
    #endregion
}
