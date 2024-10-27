/*********************************************************************************************************************
// File Name :         WeakPointHealth.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/23/24
//
// Brief Description : Controls the weak point health
*********************************************************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Weak point health deriving from BaseHealth
/// </summary>
public class WeakPointHealth : BaseHealth
{
    private UnityEvent _weakPointDamageTakenEvent = new();

    /// <summary>
    /// Applies damage to the weak point and invokes the damage event
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamage(float damage)
    {
        if(damage > 0)
        {
            _weakPointDamageTakenEvent?.Invoke();
            VfxManager.Instance.GetEnemyBloodVfx().PlayNextVfxInPool(transform.position, transform.rotation);
            base.TakeDamage(damage);
        }
    }

    #region Getters

    public UnityEvent DeathEvent() => _deathEvent;

    #endregion Getters
}
