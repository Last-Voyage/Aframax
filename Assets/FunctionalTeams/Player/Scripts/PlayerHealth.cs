/*****************************************************************************
// File Name :         PlayerHealth.cs
// Author :            Ryan Swanson
// Creation Date :     10/15/24
//
// Brief Description : Controls the player's health functionality
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player health functionality
/// </summary>
public class PlayerHealth : BaseHealth
{
    #region Base Class
    /// <summary>
    /// Inherits from BaseHealth
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="damage">The amount of damage received</param>
    protected override void HealthDecrease(float damage)
    {
        base.HealthDecrease(damage);
        PlayerManager.Instance.InvokePlayerDamagedEvent(damage);
        PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
    }

    /// <summary>
    /// Inherits from BaseHealth
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="heal">The amount of healing received</param>
    protected override void HealthIncrease(float heal)
    {
        base.HealthIncrease(heal);
        PlayerManager.Instance.InvokePlayerHealEvent(heal);
        PlayerManager.Instance.InvokePlayerHealthChangeEvent(GetHealthPercent(), _currentHealth);
    }

    /// <summary>
    /// Inherits from BaseHealth
    /// Performs the base functionality then calls player related event
    /// </summary>
    protected override void Death()
    {
        base.Death();
        PlayerManager.Instance.InvokeOnPlayerDeath();
    }
    
    #endregion

    /// <summary>
    /// This is TEMPORARY
    /// Waiting for Marks damage system
    /// Base function for taking damage is protected. Could use an event, but that falls out of my task scope
    /// </summary>
    public void TempEnemyDamage(float damage)
    {
        HealthDecrease(damage);
    }
}
