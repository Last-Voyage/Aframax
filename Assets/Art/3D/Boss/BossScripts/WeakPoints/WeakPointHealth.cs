/*********************************************************************************************************************
// File Name :         WeakPointHealth.cs
// Author :            Andrea Swihart-DeCoster
// Contributor :       Ryan Swanson
// Creation Date :     10/23/24
//
// Brief Description : Controls the weak point health
*********************************************************************************************************************/

using UnityEngine.Events;

/// <summary>
/// Weak point health deriving from BaseHealth
/// </summary>
public class WeakPointHealth : BaseHealth
{
    private readonly UnityEvent _onWeakPointDamageTakenEvent = new();

    /// <summary>
    /// Applies damage to the weak point and invokes the damage event
    /// </summary>
    /// <param name="damage"> damage amount applied to health </param>
    /// <param name="damageSource"> Incoming damage source </param>
    public override void TakeDamage(float damage, IBaseDamage damageSource)
    {
        if (damageSource == null)
        {
            return;
        }
        
        if (damageSource is HarpoonDamage)
        {
            _onWeakPointDamageTakenEvent?.Invoke();
            VfxManager.Instance.GetEnemyBloodVfx().PlayNextVfxInPool(transform.position, transform.rotation);
            base.TakeDamage(damage, null);
        }
    }
}
