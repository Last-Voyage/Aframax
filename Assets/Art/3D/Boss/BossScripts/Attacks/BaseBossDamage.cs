/*********************************************************************************************************************
// File Name :         BaseBossDamage.cs
// Author :            Ryan Swanson
// Creation Date :     11/19/24
//
// Brief Description : Controls the boss damage functionality
*********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Provides more specialized damage for the boss to deal damage to the player
/// </summary>
public class BaseBossDamage : BaseDamage
{
    /// <summary>
    /// Deals damage constantly so long as the target remains 
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerStay(Collider col)
    {
        ApplyDamage(col.gameObject);
    }

    /// <summary>
    /// Overrides the apply damage to take into account player damage cooldowns
    /// </summary>
    /// <param name="damageRecipient"></param>
    public override void ApplyDamage(GameObject damageRecipient)
    {
        //Stop if we can't apply damage
        if (!CanApplyDamage)
        {
            return;
        }

        //If the target is the player and can take damage deal damage
        //This is done to prevent multiple vfx/sfx on hit
        if (damageRecipient.TryGetComponent(out PlayerHealth health) && health.CanPlayerTakeDamage)
        {
            //Calling this instead of the base ApplyDamage as that would do TryGetComponent again
            base.ApplyDamageToHealth(health);
        }
    }
}
