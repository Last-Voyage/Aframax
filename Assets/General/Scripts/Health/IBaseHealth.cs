/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Mark Hanson
// Contributor         Ryan Swanson, Andrea Swihart-DeCoster
// Creation Date :     10/11/2024
//
// Brief Description : The modular health interface for scripts to implement
*****************************************************************************/

using UnityEngine.Events;

/// <summary>
/// The class the will be referred from to call for certain functions
/// </summary>
public interface IBaseHealth
{
    /// <summary>
    /// Initializes current health to max health
    /// </summary>
    abstract void InitializeHealth();

    /// <summary>
    /// Decreases health based on damage taken 
    /// </summary>
    /// <param name="damage"> incoming amount to decrease _currentHealth </param>
    abstract void TakeDamage(float damage);

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
    abstract void InvokeDeathEvent();

    #endregion

    #region Getters

    /// <summary>
    /// Returns health percentage
    /// </summary>
    /// <returns></returns>
    abstract float GetHealthPercent();
    abstract UnityEvent<IBaseHealth> GetDeathEvent();

    #endregion
}
