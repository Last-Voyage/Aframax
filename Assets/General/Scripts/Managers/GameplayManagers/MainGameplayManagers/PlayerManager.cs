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
    private static UnityEvent _harpoonFiredStartEvent = new();
    //When the projectile hits something or reaches its max distance
    private static UnityEvent _harpoonFiredEndEvent = new();
    //When the player presses the reel button while actively reeling
    private static UnityEvent _harpoonRetractStartEvent = new();
    //When the player releases the reel button while actively reeling
    private static UnityEvent _harpoonRetractStoppedEvent = new();
    //When the harpoon is fully reeled in
    private static UnityEvent _harpoonFullyReeledEvent = new();
    //When the harpoon is reloaded
    private static UnityEvent _harpoonReloadedEvent = new();

    //When the player starts focusing
    private static UnityEvent _harpoonFocusStartEvent = new();
    //When the player is at max focus
    private static UnityEvent _harpoonFocusMaxEvent = new();
    //When the player is no longer focused
    private static UnityEvent _harpoonFocusEndEvent = new();

    private static UnityEvent _enemyOverCrosshairStartEvent = new();
    private static UnityEvent _enemyOverCrosshairEndEvent = new();

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
    public void InvokeHarpoonFiredStartEvent()
    {
        _harpoonFiredStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the harpoon has collided with an object or reached max range
    /// </summary>
    public void InvokeHarpoonFiredEndEvent()
    {
        _harpoonFiredEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the player starts retracting the harpoon projectile
    /// </summary>
    public void InvokeHarpoonRetractStartEvent()
    {
        _harpoonRetractStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event for when the player releases the retract button while reeling
    /// </summary>
    public void InvokeHarpoonRetractStoppedEvent()
    {
        _harpoonRetractStoppedEvent?.Invoke();
    }

    public void InvokeHarpoonFullyReeledEvent()
    {
        _harpoonFullyReeledEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the harpoon focus start event
    /// </summary>
    public void InvokeHarpoonFocusStartEvent()
    {
        _harpoonFocusStartEvent?.Invoke();
    }
    
    /// <summary>
    /// Invokes the harpoon focus max event
    /// </summary>
    public void InvokeHarpoonFocusMaxEvent()
    {
        _harpoonFocusMaxEvent?.Invoke();
    }
    
    /// <summary>
    /// Invokes the harpoon focus end event
    /// </summary>
    public void InvokeHarpoonFocusEndEvent()
    {
        _harpoonFocusEndEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy start event
    /// </summary>
    public void InvokeCrosshairOverEnemyStartEvent()
    {
        _enemyOverCrosshairStartEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the crosshair over enemy end event
    /// </summary>
    public void InvokeCrosshairOverEnemyEndEvent()
    {
        _enemyOverCrosshairEndEvent?.Invoke();
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

        //when you die it reloads the current scene through the scene loading manager
        //Will be changed later to have ui pop up button to do this I would imagine
        SceneLoadingManager.Instance.DeathReloadCurrentScene();
    }
    
    #endregion

    #region Getters
    
    /// <summary>
    /// Getter for the _onMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetMovementToggleEvent() => _onMovementToggled;

    public UnityEvent GetHarpoonFiredStartEvent() => _harpoonFiredStartEvent;
    public UnityEvent GetHarpoonFiredEndEvent() => _harpoonFiredEndEvent;
    public UnityEvent GetHarpoonRetractStartEvent() => _harpoonRetractStartEvent;
    public UnityEvent GetHarpoonRetractStoppedEvent() => _harpoonRetractStoppedEvent;
    public UnityEvent GetHarpoonFullyReeledEvent() => _harpoonFullyReeledEvent;
    public UnityEvent GetHarpoonReloadedEvent() => _harpoonReloadedEvent;

    public UnityEvent GetHarpoonFocusStartEvent() => _harpoonFocusStartEvent;
    public UnityEvent GetHarpoonFocusMaxEvent() => _harpoonFocusMaxEvent;
    public UnityEvent GetHarpoonFocusEndEvent() => _harpoonFocusEndEvent;

    public UnityEvent GetEnemyOverCrosshairStartEvent() => _enemyOverCrosshairStartEvent;
    public UnityEvent GetEnemyOverCrosshairEndEvent() => _enemyOverCrosshairEndEvent;

    public UnityEvent<float> GetOnPlayerDamageEvent() => _onPlayerDamageEvent;
    public UnityEvent<float> GetOnPlayerHealEvent() => _onPlayerHealEvent;
    public UnityEvent<float, float> GetOnPlayerHealthChangeEvent() => _onHealthChange;
    public UnityEvent GetOnPlayerDeath() => _onPlayerDeath;
    
    #endregion
}
