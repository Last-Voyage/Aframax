/******************************************************************************
// File Name:       WeakPointSpawner.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster
// Creation Date:   September 22, 2024
//
// Description:     Provides the weak point with its needed functionality
******************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the weak point with its needed functionality
/// </summary>
public class WeakPoint : MonoBehaviour
{
    [Tooltip("How much health the weak point starts with and is capped at")]
    [SerializeField] private float _weakPointMaxHealth;
    private float _currentWeakPointHealth;

    private UnityEvent<float> _weakPointDamageTakenEvent = new();
    private UnityEvent _weakPointDeathEvent = new();

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
    /// Kills the weak point
    /// </summary>
    private void WeakPointDeath()
    {
        InvokeWeakPointDeathEvent();

        RuntimeSfxManager.APlayOneShotSFX?.Invoke(FmodSfxEvents.Instance.WeakPointDestroyed, transform.position);

        Destroy(gameObject);
    }

    #region Events

    /// <summary>
    /// Calls the weak point damage taken event
    /// </summary>
    /// <param name="damage"></param>
    private void InvokeWeakPointDamageTakenEvent(float damage)
    {
        _weakPointDamageTakenEvent?.Invoke(damage);
    }

    /// <summary>
    /// Calls the weak point death event
    /// </summary>
    private void InvokeWeakPointDeathEvent()
    {
        _weakPointDeathEvent?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent<float> GetWeakPointDamageTakenEvent() => _weakPointDamageTakenEvent;
    public UnityEvent GetWeakPointDeathEvent() => _weakPointDeathEvent;
    #endregion
}
