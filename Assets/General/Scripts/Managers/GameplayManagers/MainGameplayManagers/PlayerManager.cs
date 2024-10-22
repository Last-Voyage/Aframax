/******************************************************************************
// File Name:       PlayerManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides other all other scripts with access to the player
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides all other scripts with access to the player
/// </summary>
public class PlayerManager : MainGameplayManagerFramework
{
    public static PlayerManager Instance;

    /// <summary>
    /// Controls the player's movement
    /// </summary>
    private static UnityEvent<bool> _onMovementToggled = new();

    //When the projectile is fired
    private static UnityEvent _onHarpoonFiredEvent = new();
    //When the harpoon is reloaded
    private static UnityEvent _onHarpoonReloadedEvent = new();

    //When the player starts focusing
    private static UnityEvent _onHarpoonFocusStartEvent = new();
    //When the player is at max focus
    private static UnityEvent _onHarpoonFocusMaxEvent = new();
    //When the player is no longer focused
    private static UnityEvent _onHarpoonFocusEndEvent = new();

    private static UnityEvent _onEnemyOverCrosshairStartEvent = new();
    private static UnityEvent _onEnemyOverCrosshairEndEvent = new();

    //When the player takes damage
    private static UnityEvent<float> _onPlayerDamageEvent = new();
    //When the player receives healing
    private static UnityEvent<float> _onPlayerHealEvent = new();
    //When the player health changes for any reason
    private static UnityEvent<float, float> _onHealthChange = new();
    //When the player dies
    private static UnityEvent _onPlayerDeath = new();

    #region Base Manager
    public override void SetupInstance()
    {
        base.SetupInstance();
    }
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        Instance = this;
    }
    #endregion

    #region Events
    /// <summary>
    /// Invokes the _onMovementToggled event with the input bool
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    public void InvokeOnCameraMovementToggle(bool toggle)
    {
        _onMovementToggled?.Invoke(toggle);
    }

    /// <summary>
    /// Invokes event for when the harpoon projectile is fired
    /// </summary>
    public void InvokeOnHarpoonFiredEvent()
    {
        _onHarpoonFiredEvent?.Invoke();
    }

    public void InvokeOnHarpoonReloadedEvent()
    {
        _onHarpoonReloadedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the harpoon focus start event
    /// </summary>
    public void InvokeOnHarpoonFocusStartEvent()
    {
        _onHarpoonFocusStartEvent?.Invoke();
    }
    /// <summary>
    /// Invokes the harpoon focus max event
    /// </summary>
    public void InvokeOnHarpoonFocusMaxEvent()
    {
        _onHarpoonFocusMaxEvent?.Invoke();
    }
    /// <summary>
    /// Invokes the harpoon focus end event
    /// </summary>
    public void InvokeOnHarpoonFocusEndEvent()
    {
        _onHarpoonFocusEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy start event
    /// </summary>
    public void InvokeOnCrosshairOverEnemyStartEvent()
    {
        _onEnemyOverCrosshairStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy end event
    /// </summary>
    public void InvokeOnCrosshairOverEnemyEndEvent()
    {
        _onEnemyOverCrosshairEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes when the player receives damage
    /// </summary>
    /// <param name="damageTaken">The damage received</param>
    public void InvokePlayerDamagedEvent(float damageTaken)
    {
        _onPlayerDamageEvent?.Invoke(damageTaken);
    }

    /// <summary>
    /// Invokes when the player receives healing
    /// </summary>
    /// <param name="healTaken">The healing received</param>
    public void InvokePlayerHealEvent(float healTaken)
    {
        _onPlayerHealEvent?.Invoke(healTaken);
    }

    /// <summary>
    /// Invokes when the player health changes by any means
    /// </summary>
    /// <param name="percentHealth">The current percent health</param>
    /// <param name="currentHealth">The current health amount</param>
    public void InvokePlayerHealthChangeEvent(float percentHealth, float currentHealth)
    {
        _onHealthChange?.Invoke(percentHealth, currentHealth);
    }

    /// <summary>
    /// Invokes when the player dies
    /// </summary>
    public void InvokeOnPlayerDeath()
    {
        _onPlayerDeath?.Invoke();
        SceneLoadingManager.Instance.DeathReloadCurrentScene();
    }
    #endregion

    #region Getters
    /// <summary>
    /// Getter for the _onMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetOnMovementToggleEvent() => _onMovementToggled;

    public UnityEvent GetOnHarpoonFiredEvent() => _onHarpoonFiredEvent;
    public UnityEvent GetOnHarpoonReloadedEvent() => _onHarpoonReloadedEvent;

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
