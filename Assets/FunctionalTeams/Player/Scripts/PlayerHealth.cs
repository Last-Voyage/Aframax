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
    public static PlayerHealth Instance;

    //determins if the player should take damage
    //This was added by Nabil for implementing god mode
    public bool _shouldTakeDamage = true;

    #region Base Class
    //Performs the base starting functionality and sets the instance
    protected override void Awake()
    {
        EstablishInstance();
        base.Awake();
    }

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
    /// Establishes the instance and removes
    /// </summary>
    private void EstablishInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This is TEMPORARY
    /// Waiting for Marks damage system
    /// Base function for taking damage is protected. Could use an event, but that falls out of my task scope
    /// </summary>
    public void TempEnemyDamage(float damage)
    {
        if (_shouldTakeDamage) { HealthDecrease(damage); }
        
    }
}
