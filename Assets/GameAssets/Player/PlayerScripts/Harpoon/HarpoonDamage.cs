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
        //Checks if its an object to spawn vfx on
        if (damageRecipient.TryGetComponent<HarpoonHitVfxSpawner>(out HarpoonHitVfxSpawner harpoonHitVfx))
        {
            harpoonHitVfx.HarpoonHit(transform);
        }

        // Avoids damaging the player
        if (damageRecipient.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)
            || !damageRecipient.CompareTag("Enemy")) 
        {
            return;
        }
        base.ApplyDamage(damageRecipient);
    }
}
