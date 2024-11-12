/*********************************************************************************************************************
// File Name :         BoatGeometryHealth
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/28/24
//
// Brief Description : Manages the health for a single portion of the boat.
*********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Manages the health for a single portion of the boat.
/// </summary>
public class BoatGeometryHealth : BaseHealth
{
    /// <summary>
    /// Overrides the base TakeDamage script to play a specific sfx is the
    /// boat was hit by the harpoon.
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
                PlayOnHitSFX();
            }
        }
    }

    /// <summary>
    /// Plays the onHit sfx if hit by the harpoon
    /// </summary>
    private void PlayOnHitSFX()
    {
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.HarpoonHitBoat, transform.position);
    }
}
