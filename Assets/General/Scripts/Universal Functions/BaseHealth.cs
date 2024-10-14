/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Mark Hanson
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
    protected int _currentHealth;
    protected int _maxHealth;
    //Event system used to be called outside of script
    private UnityEvent<BaseHealth> _deathEvent = new ();
    /// <summary>
    /// Function for decreasing health 
    /// </summary>
    /// <param name="damage"></param>
    protected virtual void HealthDecrease(int damage)
    {
        _currentHealth -= damage;
        //Check if health is lower than 0 then destroys the gameObject attached
        if ( _currentHealth <= 0)
        {
            InvokeDeathEvent();
        }
    }
    /// <summary>
    /// function of health increasing
    /// </summary>
    /// <param name="heal"></param>
    protected virtual void HealthIncrease(int heal)
    {
        _currentHealth += heal;
        //Check if health is higher than max health then return back down to max heal
        if(_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
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
    public UnityEvent<BaseHealth> GetDeathEvent() => _deathEvent;
    #endregion

}
