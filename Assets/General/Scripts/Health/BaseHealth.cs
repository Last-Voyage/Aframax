/*****************************************************************************
// File Name :         BaseHealth.cs
// Author :            Ryan Swanson
// Contributors:       Andrea Swihart-DeCoster
// Creation Date :     10/15/24
//
// Brief Description : Controls the health functionality
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls the player health functionality
/// </summary>
public class BaseHealth : MonoBehaviour, IBaseHealth
{
    [Header("Maximum Health On Start")]
    [SerializeField] protected float _maxHealth;

    /// <summary>
    /// Current health value
    /// </summary>
    protected float _currentHealth;

    //Event system used to be called outside of script
    protected UnityEvent<IBaseHealth> _deathEvent = new();

    #region Base Class

    private void Awake()
    {
        InitializeHealth();
    }

    public void InitializeHealth()
    {
        _currentHealth = _maxHealth;
    }

    /// <summary>
    /// Inherits from BaseHealth
    /// Performs the base functionality then calls player related event
    /// </summary>
    /// <param name="heal">The amount of healing received</param>
    public virtual void IncreaseHealth(float heal)
    {
        _currentHealth += heal;

        //Check if health is higher than max health then return back down to max heal
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    /// <summary>
    /// Inherits from BaseHealth
    /// Performs the base functionality then calls player related event
    /// </summary>
    public virtual void OnDeath()
    {
        InvokeDeathEvent();
    }

    #endregion

    /// <summary>
    /// This is TEMPORARY
    /// Waiting for Marks damage system
    /// Base function for taking damage is protected. Could use an event, but that falls out of my task scope
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            OnDeath();
        }
    }

    #region Events

    /// <summary>
    /// Death event to kill gameObject from the object
    /// </summary>
    public void InvokeDeathEvent()
    {
        _deathEvent?.Invoke(this);
    }

    #endregion

    #region Getters

    public float GetHealthPercent() => _currentHealth / _maxHealth;
    public UnityEvent<IBaseHealth> GetDeathEvent() => _deathEvent;

    #endregion Getters
}
