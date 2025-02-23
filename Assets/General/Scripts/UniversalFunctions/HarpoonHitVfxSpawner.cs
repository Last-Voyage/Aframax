/*********************************************************************************************************************
// File Name :         HarpoonHitVfxSpawner.cs
// Author :            Ryan Swanson
// Creation Date :     02/23/2025
//
// Brief Description : Manages the VFX of hitting the environment with a harpoon
*********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// The types of environment that can be hit by a harpoon
/// </summary>
public enum HarpoonEnvironmentHitTypes
{
    Wood,
    Metal
}

/// <summary>
/// Spawns vfx when hit by a harpoon
/// </summary>
public class HarpoonHitVfxSpawner : MonoBehaviour
{
    [SerializeField] private HarpoonEnvironmentHitTypes _hitType;

    private SpecificVisualEffect _associatedEffect;

    /// <summary>
    /// Assigns the associated effect based on the environment type
    /// </summary>
    public void Start()
    {
        switch (_hitType)
        {
            case (HarpoonEnvironmentHitTypes.Wood):
                _associatedEffect = VfxManager.Instance.GetWoodenSparksVfx();
                break;
            case (HarpoonEnvironmentHitTypes.Metal):
                _associatedEffect = VfxManager.Instance.GetMetalSparksVfx();
                break;
        }
    }

    /// <summary>
    /// Called by the harpoon on
    /// </summary>
    /// <param name="harpoonOrientation"> The world transform of the harpoon </param>
    public void HarpoonHit(Transform harpoonOrientation)
    {
        _associatedEffect.PlayNextVfxInPool(harpoonOrientation.transform.position,harpoonOrientation.transform.rotation);
    }
}
