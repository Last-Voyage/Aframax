/******************************************************************************
// File Name:       SpannerInteractable.cs
// Author:          Ryan Swanson
// Contributor:     Mark Hanson
// Creation Date:   October 12th, 2024
//
// Description:     Contains the functionality for the spanner interactable
******************************************************************************/

using UnityEngine;

/// <summary>
/// Provides the functionality for the player to pickup the spanner
/// </summary>
public class SpannerInteractable : MonoBehaviour, IPlayerInteractable
{
    public bool InteractEnabled { get; set; }

    private void Awake()
    {
        InteractEnabled = true;
    }
    
    /// <summary>
    /// Switch toggle for if interaction availability
    /// </summary>
    public void CanBeInteractedWith()
    {
        if (InteractEnabled)
        {
            InteractEnabled = false;
        }

        if (!InteractEnabled)
        {
            InteractEnabled = true;
        }
    }
    
    /// <summary>
    /// Implements the OnInteractedByPlayer function from IPlayerInteractable
    /// Called when the player presses the interact key while looking at this object
    /// </summary>
    public void OnInteractedByPlayer()
    {
        PlayerInventory.Instance.DoesPlayerHaveSpanner = true;
        //TODO: I imagine there will be an animation or vfx for this in the future, but for now we just destroy it
        Destroy(gameObject);
    }
}
