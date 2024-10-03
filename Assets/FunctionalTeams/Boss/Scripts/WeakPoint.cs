/******************************************************************************
// File Name:       WeakPointSpawner.cs
// Author:          Ryan Swanson
// Creation Date:   September 22, 2024
//
// Description:     Provides the weakpoint with its needed functionality
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the weakpoint with its needed functionality
/// </summary>
public class WeakPoint : MonoBehaviour
{
    [Tooltip("How much health the weak point starts with and is capped at")]
    [SerializeField] private float _weakPointMaxHealth;
    private float _currentWeakPointHealth;

    private UnityEvent<float> _weakPointDamageTakenEvent = new();
    private UnityEvent<WeakPoint> _weakPointDeathEvent = new();

    private void Start()
    {
        _currentWeakPointHealth = _weakPointMaxHealth;

    }

    /// <summary>
    /// The function through which all damage dealt to weakpoints passes through
    /// </summary>
    /// <param name="damage"></param>
    public void DamageWeakPoint(float damage)
    {
        //Decreases health
        _currentWeakPointHealth -= damage;
        //Invokes damage event
        InvokeWeakPointDamageTakenEvent(damage);
        //Checks for death
        CheckForWeakPointDeath();
    }

    /// <summary>
    /// Checks if the weak point has died
    /// </summary>
    private void CheckForWeakPointDeath()
    {
        //Check if health is less than 0
        if (_currentWeakPointHealth <= 0)
        {
            WeakPointDeath();
        }
    }

    /// <summary>
    /// Kills the weakpoint
    /// </summary>
    private void WeakPointDeath()
    {
        InvokeWeakPointDeathEvent();
        Destroy(gameObject);
    }

    #region Events
    private void InvokeWeakPointDamageTakenEvent(float damage)
    {
        _weakPointDamageTakenEvent?.Invoke(damage);
    }

    private void InvokeWeakPointDeathEvent()
    {
        _weakPointDeathEvent?.Invoke(this);
    }
    #endregion

    #region Getters
    public UnityEvent<float> GetWeakPointDamageTakenEvent() => _weakPointDamageTakenEvent;
    public UnityEvent<WeakPoint> GetWeakPointDeathEvent() => _weakPointDeathEvent;
    #endregion
}
