/*********************************************************************************************************************
// File Name :         MetalBoatHealth.cs
// Author :            Mark Hanson
// Creation Date :     11/14/2024
//
// Brief Description : Manages the VFX of hitting the boat with the harpoon
*********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages the VFX of hitting the boat with the harpoon
/// </summary>
public class MetalBoatHealth : BaseHealth
{
    /// <summary>
    /// Overrides the base TakeDamage script to play the Wooden Spark VFX
    /// </summary>
    /// <param name="damage">amount of incoming damage</param>
    /// <param name="damageSource">incoming damage source</param>
    public override void TakeDamage(float damage, IBaseDamage damageSource)
    {
        base.TakeDamage(damage, damageSource);
        if (damageSource != null)
        {
            if (damageSource is HarpoonDamage)
            {
                PlayOnHitVfx();
            }
        }
    }

    /// <summary>
    ///  Plays the on Hit VFX if hit by the harpoon
    /// </summary>
    private void PlayOnHitVfx()
    {
        VfxManager.Instance.GetMetalSparksVfx()
            .PlayNextVfxInPool(BoatMover.Instance.transform, transform.position, transform.rotation);
    }
}
