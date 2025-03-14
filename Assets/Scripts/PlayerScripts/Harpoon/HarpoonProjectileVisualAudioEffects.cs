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
using FMODUnity;

/// <summary>
/// The types of objects that the harpoon collides with
/// </summary>
[Tooltip("Which VFX will be spawned")]
public enum HarpoonCollisionType : uint
{
    NOVFX,
    SPARKVFX,
    WOODVFX,
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

    private static EventReference[] _harpoonAudioEffects = new EventReference[4];

    [Tooltip("The pointer that reflects the vfx type")]
    private uint _whichVfxPointer;

    private HarpoonProjectileMovement _associatedMovement;

    /// <summary>
    /// Performs set up for the harpoon effects
    /// </summary>
    private void Awake()
    {
        if (_harpoonVisualEffects.Length == 0 || _harpoonVisualEffects.IsUnityNull()
            || _materialVfxRefs.Count == 0 || _materialVfxRefs.IsUnityNull())
        {
            InitializeHarpoonVisualEffects();
            InitializeHarpoonAudioEffects();

            InitializeVisualEffectsDictionary();
        }
        SetStartingValues();
    }

    /// <summary>
    /// Performs the collision check for objects we create harpoon effects on
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        #region Checks and variables for the collision
        //Check if what we hit has a mesh renderer and collider
        // and therefore requires more specific checks for the material
        if (other.gameObject.TryGetComponent<MeshCollider>(out var anotherCollider)
            && other.gameObject.TryGetComponent<MeshRenderer>(out var anotherRenderer))
        {
            HarpoonMeshCollision(anotherCollider, anotherRenderer);
        }
        //Otherwise use the regular collision check
        else if (other.gameObject.TryGetComponent<Renderer>(out var contactRenderer))
        {
            HarpoonGeneralCollision(contactRenderer);
        }
        //We hit something without a renderer so there is no point playing vfx
        else
        {
            return;
        }

        // Checks if the harpoon hit a material that would cause a Vfx to spawn
        if (_harpoonVisualEffects[_whichVfxPointer] != null)
        {
            SpawnProjectileVfx();
            StartProjectileImpactSfx();
            _associatedMovement.IsHit = true;
        }
    }

    /// <summary>
    /// Performs a check of an object with a mesh collider to determine what material we are hitting
    /// </summary>
    /// <param name="meshCollider">The collider it hit</param>
    /// <param name="meshRenderer">The renderer object attached to what it hit</param>
    private void HarpoonMeshCollision(MeshCollider meshCollider, MeshRenderer meshRenderer)
    {
        // Mesh reference & submesh count
        Mesh theCollidedMesh = meshCollider.sharedMesh;
        int howManyMeshes = theCollidedMesh.subMeshCount;
        Material[] collidedObjectsMaterials = meshRenderer.sharedMaterials;
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
        _whichVfxPointer = (uint)HarpoonCollisionType.NOVFX;
        // Goes through each submesh to check if the raycast collided with an index within it's value range
        for (int i = 0; i < howManyMeshes; i++)
        {
            SubMeshDescriptor subMesh = theCollidedMesh.GetSubMesh(i);

            // In the event that the triangle check fails but we still get the correct collision object
            // This is working even for objects with multiple different materials attached
            if (_materialVfxRefs.TryGetValue(collidedObjectsMaterials[i], out uint usableVfxPointer))
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
    }

    /// <summary>
    /// Performs a check of an object to determine what material we hit
    /// </summary>
    /// <param name="renderer"> The renderer of the object we hit </param>
    private void HarpoonGeneralCollision(Renderer renderer)
    {
        // Iterates through each material
        foreach (Material mat in renderer.sharedMaterials)
        {
            //Checks if that material has a hit vfx associated
            if (_materialVfxRefs.TryGetValue(mat, out uint usableVfxPointer))
            {
                _whichVfxPointer = usableVfxPointer;
                return;
            }
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
    /// Plays a sfx where the harpoon hit
    /// </summary>
    private void StartProjectileImpactSfx()
    {
        RuntimeSfxManager.APlayOneShotSfx.Invoke(_harpoonAudioEffects[_whichVfxPointer], gameObject.transform.position);
    }

    /// <summary>
    /// Grabs all the referenceable vfx and puts it into an array
    /// </summary>
    private void InitializeHarpoonVisualEffects()
    {
        _harpoonVisualEffects[(uint)HarpoonCollisionType.NOVFX] = null;
        _harpoonVisualEffects[(uint)HarpoonCollisionType.SPARKVFX] = VfxManager.Instance.GetMetalSparksVfx();
        _harpoonVisualEffects[(uint)HarpoonCollisionType.WOODVFX] = VfxManager.Instance.GetWoodenSparksVfx();
    }

    /// <summary>
    /// Grabs all the referenceable sfx and puts it in an array
    /// </summary>
    private void InitializeHarpoonAudioEffects()
    {
        _harpoonAudioEffects[(uint)HarpoonCollisionType.NOVFX] = FmodSfxEvents.Instance.HarpoonHitGeneral;
        _harpoonAudioEffects[(uint)HarpoonCollisionType.SPARKVFX] = FmodSfxEvents.Instance.HarpoonHitMetal;
        _harpoonAudioEffects[(uint)HarpoonCollisionType.WOODVFX] = FmodSfxEvents.Instance.HarpoonHitWood;
    }

    /// <summary>
    /// Sets any values needed for start
    /// </summary>
    private void SetStartingValues()
    {
        TryGetComponent(out _associatedMovement);
    }

    /// <summary>
    /// Adds the materials to the vfx dictionary
    /// </summary>
    private void InitializeVisualEffectsDictionary()
    {
        foreach (HarpoonVisualAudioEffectsBank bank in VfxManager.Instance.GetHarpoonVisualArray())
        {
            if (bank.AssociatedMaterial == null)
            {
                continue;
            }
            _materialVfxRefs.Add(bank.AssociatedMaterial, (uint)bank.CollisionType);
        }
    }
}