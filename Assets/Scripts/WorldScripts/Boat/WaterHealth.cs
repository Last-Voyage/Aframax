/*********************************************************************************************************************
// File Name :         WaterHealth
// Author :            Mark Hanson
// Creation Date :     11/12/2024
//
// Brief Description : Manages the health of the water from harpoons
*********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Manages the health for the water from harpoons
/// </summary>
public class WaterHealth : BaseHealth
{
    /// <summary>
    /// Overrides the base TakeDamage script to play a specific sfx is the
    /// Water splashes from the harpoon.
    /// </summary>
    /// <param name="damage"> amount of incoming damage </param>
    /// <param name="damageSource"> incoming damage source </param>
    public override void TakeDamage(float damage, IBaseDamage damageSource)
    {
        base.TakeDamage(damage, damageSource);

        if (damageSource != null)
        {
            if (damageSource is HarpoonDamage)
            {
                PlayOnHitSfx();
            }
        }
    }

    /// <summary>
    /// Plays the onHit sfx if hit by the harpoon
    /// </summary>
    private void PlayOnHitSfx()
    {
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.HarpoonWaterSplash, transform.position);
    }
}
