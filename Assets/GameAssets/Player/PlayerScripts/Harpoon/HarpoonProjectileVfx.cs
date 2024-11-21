/******************************************************************************
// File Name:       HarpoonProjectileVfx.cs
// Author:          Nick Rice
// Creation Date:   November 17th, 2024
//
// Description:     Handles the harpoon projectile hit vfx spawning
******************************************************************************/
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Handles the harpoon projectile hit vfx spawning
/// </summary>
public class HarpoonProjectileVfx : MonoBehaviour
{
    [Tooltip("The dictionary for materials and vfx")]
    private Dictionary<Material, uint> _materialVfxRefs = new();

    [Tooltip("The array of vfx")]
    private SpecificVisualEffect[] _harpoonVisualEffects = new SpecificVisualEffect[4];

    [Tooltip("Materials that, when hit, cause a vfx effect")]
    [SerializeField]
    private Material[] _vfxCollisionMaterials;

    [Tooltip("Which VFX will be spawned")]
    private enum VFXType : uint
    {
        NOVFX,
        SPARKVFX,
        DECKVFX,
        TREEVFX
    }

    [Tooltip("The pointer that reflects the vfx type")]
    private uint _whichVfxPointer;

    private void Awake()
    {
        InitializeHarpoonVisualEffects();
        
        InitializeVisualEffectsDictionary();
    }

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
        _whichVfxPointer = (uint)VFXType.NOVFX;
        
        // Goes through each submesh to check if the raycast collided with an index within it's value range
        for (int i = 0; i < howManyMeshes; i++)
        {
            SubMeshDescriptor subMesh = theCollidedMesh.GetSubMesh(i);

            // If the index is in the value range,
            // that material/submesh needs to match a possible vfx in the dictionary
            if (whichTriangle > subMesh.indexStart && whichTriangle < subMesh.indexCount && 
                _materialVfxRefs.TryGetValue(collidedObjectsMaterials[i], out uint usableVfxPointer))
            {
                _whichVfxPointer = usableVfxPointer;
                break;
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
        _harpoonVisualEffects[(uint)VFXType.NOVFX] = null;
        _harpoonVisualEffects[(uint)VFXType.SPARKVFX] = VfxManager.Instance.GetMetalSparksVfx();
        _harpoonVisualEffects[(uint)VFXType.DECKVFX] = VfxManager.Instance.GetWoodenSparksVfx();
        _harpoonVisualEffects[(uint)VFXType.TREEVFX] = VfxManager.Instance.GetTreeSplintersVfx();
    }

    /// <summary>
    /// Adds the materials to the vfx dictionary
    /// </summary>
    private void InitializeVisualEffectsDictionary()
    {
        _materialVfxRefs.Add(_vfxCollisionMaterials[0], (uint)VFXType.SPARKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[1], (uint)VFXType.SPARKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[2], (uint)VFXType.DECKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[3], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[4], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[5], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[6], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[7], (uint)VFXType.TREEVFX);
    }
}