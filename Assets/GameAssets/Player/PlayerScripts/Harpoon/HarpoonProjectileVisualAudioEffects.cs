/******************************************************************************
// File Name:       HarpoonProjectileVisualAudioEffects.cs
// Author:          Nick Rice
// Contributors:    Ryan Swanson
// Creation Date:   November 17th, 2024
//
// Description:     Handles the harpoon projectile hit vfx spawning
******************************************************************************/
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// The types of objects that the harpoon collides with
/// </summary>
[Tooltip("Which VFX will be spawned")]
public enum HarpoonVFXType : uint
{
    NOVFX,
    SPARKVFX,
    DECKVFX,
}

/// <summary>
/// Handles the harpoon projectile hit vfx spawning
/// </summary>
public class HarpoonProjectileVisualAudioEffects : MonoBehaviour
{
    [Tooltip("The dictionary for materials and vfx")]
    private static Dictionary<Material, uint> _materialVfxRefs = new();

    [Tooltip("The array of vfx")]
    private static SpecificVisualEffect[] _harpoonVisualEffects = new SpecificVisualEffect[4];

    [Tooltip("The pointer that reflects the vfx type")]
    private uint _whichVfxPointer;

    /// <summary>
    /// Performs set up for the harpoon effects
    /// </summary>
    private void Awake()
    {
        if (_harpoonVisualEffects.Length == 0 || _harpoonVisualEffects.IsUnityNull()
            || _materialVfxRefs.Count == 0 || _materialVfxRefs.IsUnityNull())
        {
            InitializeHarpoonVisualEffects();

            InitializeVisualEffectsDictionary();
        }
    }

    /// <summary>
    /// Performs the collision check for objects we create harpoon effects on
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        #region Checks and variables for the collision
        // If the collided object does not have these components, it will exit this function
        // If it does, the rest of the script can use the "out"put variable
        if (!other.gameObject.TryGetComponent<MeshCollider>(out var anotherCollider)
            || !other.gameObject.TryGetComponent<MeshRenderer>(out var anotherRenderer))
        {
            return;
        }
        // Mesh reference & submesh count
        Mesh theCollidedMesh = anotherCollider.sharedMesh;
        int howManyMeshes = theCollidedMesh.subMeshCount;
        Material[] collidedObjectsMaterials = anotherRenderer.sharedMaterials;
        #endregion

        #region Checks and variables for the raycast
        // The raycast
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward),
            out RaycastHit submeshHitPoint, 1f);

        // A check to see if the ray collided, and if so, 
        if (submeshHitPoint.collider.IsUnityNull())
        {
            return;
        }
        // The reference to the triangle hit in the submesh
        int whichTriangle = submeshHitPoint.triangleIndex;
        #endregion

        // Sets the vfx to null; if there isn't an associated vfx with what was hit, then, nothing happens
        _whichVfxPointer = (uint)HarpoonVFXType.NOVFX;
        // Goes through each submesh to check if the raycast collided with an index within it's value range
        for (int i = 0; i < howManyMeshes; i++)
        {
            SubMeshDescriptor subMesh = theCollidedMesh.GetSubMesh(i);

            // In the event that the triangle check fails but we still get the correct collision object
            // This is working even for objects with multiple different materials attached
            if(_materialVfxRefs.TryGetValue(collidedObjectsMaterials[i], out uint usableVfxPointer))
            {
                _whichVfxPointer = usableVfxPointer;
                // If the index is in the value range,
                // that material/submesh needs to match a possible vfx in the dictionary
                if (whichTriangle >= subMesh.indexStart && whichTriangle <= subMesh.indexCount)
                {
                    break;
                }    
            }
        }

        // Checks if the harpoon hit a material that would cause a Vfx to spawn
        if (_harpoonVisualEffects[_whichVfxPointer] != null)
        {
            SpawnProjectileVfx();
        }
    }

    /// <summary>
    /// Spawns a vfx where the harpoon hit
    /// </summary>
    private void SpawnProjectileVfx()
    {
        _harpoonVisualEffects[_whichVfxPointer].PlayNextVfxInPool
            (gameObject.transform.position, Quaternion.Inverse(gameObject.transform.rotation));
    }

    /// <summary>
    /// Grabs all the referenceable vfx and puts it into an array
    /// </summary>
    private void InitializeHarpoonVisualEffects()
    {
        _harpoonVisualEffects[(uint)HarpoonVFXType.NOVFX] = null;
        _harpoonVisualEffects[(uint)HarpoonVFXType.SPARKVFX] = VfxManager.Instance.GetMetalSparksVfx();
        _harpoonVisualEffects[(uint)HarpoonVFXType.DECKVFX] = VfxManager.Instance.GetWoodenSparksVfx();
    }

    /// <summary>
    /// Adds the materials to the vfx dictionary
    /// </summary>
    private void InitializeVisualEffectsDictionary()
    {
        foreach(HarpoonVisualAudioEffectsBank bank in VfxManager.Instance.GetHarpoonVisualArray())
        {
            if(bank._associatedMaterial == null)
            {
                continue;
            }
            _materialVfxRefs.Add(bank._associatedMaterial, (uint)bank._vfxType);
        }
    }
}