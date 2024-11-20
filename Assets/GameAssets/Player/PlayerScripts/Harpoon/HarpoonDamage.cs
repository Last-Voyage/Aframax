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
    //Applies damage to the recipient as long as it is not the player
    /// </summary>
    /// <param name="damageRecipient"></param>
    public override void ApplyDamage(GameObject damageRecipient)
    {
        // Avoids damaging the player
        if (damageRecipient.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }

        base.ApplyDamage(damageRecipient);
        GetComponent<HarpoonProjectileMovement>().ShouldHarpoonStick(damageRecipient);
    }
}
