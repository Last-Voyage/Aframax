/**********************************************************************************************************************
// File Name :         GeneratorInteractable.cs
// Author :            Ryan Swanson
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
