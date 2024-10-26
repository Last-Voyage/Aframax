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
    [SerializeField] protected float _maxHealth;

    /// <summary>
    /// Current health value
    /// </summary>
    protected float _currentHealth;

    //Event system used to be called outside of script
    protected UnityEvent _deathEvent = new();

    #region Base Class

    protected void Awake()
    {
        InitializeHealth(_maxHealth);
    }

    /// <summary>
    /// Initializes any starter health values
    /// </summary>
    public void InitializeHealth(float healthValue)
    {
        if(healthValue > _maxHealth)
        {
            healthValue = Mathf.Clamp(healthValue, healthValue, _maxHealth);
        }
        _currentHealth = healthValue;
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
    /// Performs the base functionality then the general death event
    /// </summary>
    public virtual void OnDeath() 
    { 
        _deathEvent.Invoke();
    }

    #endregion

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
        _deathEvent?.Invoke();
    }

    #endregion

    #region Getters

    public float GetHealthPercent() => _currentHealth / _maxHealth;
    public UnityEvent GetDeathEvent() => _deathEvent;

    #endregion Getters
}
