/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Mark Hanson
// Contributor         Ryan Swanson, Andrea Swihart-DeCoster
// Creation Date :     10/11/2024
//
// Brief Description : The modular health interface for scripts to implement
*****************************************************************************/

using UnityEngine.Events;
using UnityEngine;

/// <summary>
/// The interface for all health functionality.
/// </summary>
public interface IBaseHealth
{
    /// <summary>
    /// Initializes current health to max health
    /// </summary>
    abstract void InitializeHealth(float healthValue);

    /// <summary>
    /// Decreases health based on damage taken 
    /// </summary>
    /// <param name="damage"> incoming amount to decrease _currentHealth </param>
    /// <param name="damageSource"> Source of the incoming damage. Typically input as "this" </param>
    abstract void TakeDamage(float damage, IBaseDamage damageSource);

    /// <summary>
    /// function of health increasing
    /// </summary>
    /// <param name="heal"></param>
    abstract void IncreaseHealth(float heal);

    /// <summary>
    /// Function called when the current health reaches 0
    /// Invokes death event
    /// </summary>
    abstract void OnDeath();

    #region Events

    /// <summary>
    /// Calls the Death event
    /// </summary>
    abstract void OnInvokeDeathEvent();

    #endregion

    #region Getters

    /// <summary>
    /// Access the current health percentage
    /// </summary>
    /// <returns> Current health percentage </returns>
    abstract float GetHealthPercent();

    /// <summary>
    /// Access death event
    /// </summary>
    abstract UnityEvent GetOnDeathEvent();

    #endregion
}
