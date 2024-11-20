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

    [Tooltip("Moves the sparks and smoke forwards")]
    private Vector3 aBoost = new Vector3(0, 0, -1);
    
    /// <summary>
    /// This function spawns in the smoke for the generator
    /// </summary>
    public void GeneratorStartsSmoking()
    {
        
        VfxManager.Instance.GetPlumeSmokeVfx().PlayNextVfxInPool
            (transform.position + aBoost, transform.rotation);
    }

    /// <summary>
    /// This functions spawns in the sparks for the generator
    /// </summary>
    public void GeneratorSparks()
    {
        VfxManager.Instance.GetMetalSparksVfx().PlayNextVfxInPool
            (transform.position + aBoost, transform.rotation);
    }

    public void OnInteractedByPlayer()
    {
        if(!CanGeneratorBeInteracted)
        {
            return;
        }

        if(DoesRequireSpanner && !PlayerInventory.Instance.DoesPlayerHaveSpanner)
        {
            return;
        }

        StoryManager.Instance.ProgressNextStoryBeat();
    }
}
