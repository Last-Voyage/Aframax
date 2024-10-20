/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Mark Hanson
//                     Ryan Swanson
// Creation Date :     10/11/2024
//
// Brief Description : The universal health system used to be derived from
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// The class the will be referred from to call for certain functions
/// </summary>
public class BaseHealth : MonoBehaviour
{
    [SerializeField] protected float _maxHealth;
    protected float _currentHealth;
    
    //Event system used to be called outside of script
    private UnityEvent<BaseHealth> _deathEvent = new ();

    /// <summary>
    /// Sets initial values
    /// </summary>
    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    /// <summary>
    /// Function for decreasing health 
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void HealthDecrease(float damage)
    {
        _currentHealth -= damage;
        //Check if health is lower than 0 then destroys the gameObject attached
        if ( _currentHealth <= 0)
        {
            Death();
        }
    }
    /// <summary>
    /// function of health increasing
    /// </summary>
    /// <param name="heal"></param>
    protected virtual void HealthIncrease(float heal)
    {
        _currentHealth += heal;
        //Check if health is higher than max health then return back down to max heal
        if(_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    /// <summary>
    /// Function called when the current health reaches 0
    /// Invokes death event
    /// </summary>
    protected virtual void Death()
    {
        InvokeDeathEvent();
    }
    #region Events
    /// <summary>
    /// Death event to kill gameObject from the object
    /// </summary>
    private void InvokeDeathEvent()
    {
        _deathEvent?.Invoke(this);
    }
    #endregion
    #region Getters
    public float GetHealthPercent() => _currentHealth / _maxHealth;
    public UnityEvent<BaseHealth> GetDeathEvent() => _deathEvent;
    #endregion

}
