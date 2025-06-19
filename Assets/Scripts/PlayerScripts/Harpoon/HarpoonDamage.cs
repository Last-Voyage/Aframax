/*********************************************************************************************************************
// File Name :         HarpoonDamage.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/24/24
//
// Brief Description : Controls the harpoon damage functionality
*********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Extends from BaseDamage and controls the harpoon damage functionality.
/// </summary>
public class HarpoonDamage : BaseDamage
{
    /// <summary>
    /// Applies damage to the recipient as long as it is not the player
    /// </summary>
    /// <param name="damageRecipient"></param>
    public override void ApplyDamage(GameObject damageRecipient)
    {
        // Avoids damaging the player
        if (damageRecipient.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth) || !damageRecipient.CompareTag("Enemy"))
        {
            return;
        }
        base.ApplyDamage(damageRecipient);
    }

    /// <summary>
    /// Overrides the ApplyDamageToHealth function to disable the harpoon after it deals damage
    /// </summary>
    /// <param name="health"> The health of the target hit </param>
    protected override void ApplyDamageToHealth(IBaseHealth health)
    {
        base.ApplyDamageToHealth(health);
        // Prevents harpoon front sticking into something we damage
        gameObject.SetActive(false);
    }
}
