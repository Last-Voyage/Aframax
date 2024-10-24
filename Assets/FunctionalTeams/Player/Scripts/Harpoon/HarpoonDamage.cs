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
    public override void ApplyDamage(GameObject damageRecipient)
    {
        // Avoids damaging the player
        if (damageRecipient.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }
        
        base.ApplyDamage(damageRecipient);
    }
}
