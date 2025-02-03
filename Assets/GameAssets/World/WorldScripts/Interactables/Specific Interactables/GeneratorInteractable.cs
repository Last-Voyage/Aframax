/**********************************************************************************************************************
// File Name :         GeneratorInteractable.cs
// Author :            Ryan Swanson
// Contributors:       Nick Rice
// Creation Date :     11/14/24
// 
// Brief Description : Controls the functionality for the generator
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the functionality for the generator
/// </summary>
public class GeneratorInteractable : MonoBehaviour, IPlayerInteractable
{
    public bool CanGeneratorBeInteracted { get; set; }
    public bool DoesRequireSpanner { get; set; }

    [Tooltip("Moves the sparks and smoke forwards, or elsewhere")]
    private readonly Vector3 _vfxDisplacement = Vector3.back;
    
    /// <summary>
    /// This function spawns in the smoke for the generator
    /// </summary>
    public void GeneratorStartsSmoking()
    {
        VfxManager.Instance.GetPlumeSmokeVfx().PlayNextVfxInPool
            (transform.position + _vfxDisplacement, transform.rotation);
    }

    /// <summary>
    /// This functions spawns in the sparks for the generator
    /// </summary>
    public void GeneratorSparks()
    {
        VfxManager.Instance.GetMetalSparksVfx().PlayNextVfxInPool
            (transform.position + _vfxDisplacement, transform.rotation);
    }

    /// <summary>
    /// When the generator is interacted with by the player pressing the interact key on it.
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if(!CanGeneratorBeInteracted)
        {
            return;
        }

        if(DoesRequireSpanner)
        {
            if(PlayerInventory.Instance.DoesPlayerHaveSpanner)
            {
                StoryManager.Instance.ProgressNextStoryBeat();
                RuntimeSfxManager.APlayOneShotSfxAttached?.Invoke(FmodSfxEvents.Instance.GeneratorFixed, gameObject);
            }
        }
        else
        {
            StoryManager.Instance.ProgressNextStoryBeat();
        }

        
    }
}
