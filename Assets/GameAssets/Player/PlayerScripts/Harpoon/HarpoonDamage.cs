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
    /// Overrides the on trigger enter to also check for Vfx
    /// </summary>
    /// <param name="col"> The collider we contacted </param>
    protected override void OnTriggerEnter(Collider col)
    {
        CheckForVfxObject(col.gameObject);
        base.OnTriggerEnter(col);
    }

    /// <summary>
    /// Applies damage to the recipient as long as it is not the player
    /// </summary>
    /// <param name="damageRecipient"> The target to deal damage to</param>
    public override void ApplyDamage(GameObject damageRecipient)
    {
        // Avoids damaging the player
        if (damageRecipient.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)
            || !damageRecipient.CompareTag("Enemy")) 
        {
            return;
        }
        base.ApplyDamage(damageRecipient);
    }

    /// <summary>
    /// Checks if the object should spawn Vfx on collision
    /// </summary>
    /// <param name="target"> The target for collision </param>
    private void CheckForVfxObject(GameObject target)
    {
        //Checks if its an object to spawn vfx on
        if (target.TryGetComponent<HarpoonHitVfxSpawner>(out HarpoonHitVfxSpawner harpoonHitVfx))
        {
            harpoonHitVfx.HarpoonHit(transform);
        }
    }
}
