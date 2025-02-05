/******************************************************************************
// File Name:       SpannerInteractable.cs
// Author:          Ryan Swanson
// Contributer:     Charlie Polonus
// Creation Date:   October 12th, 2024
//
// Description:     Contains the functionality for the spanner interactable
******************************************************************************/

using UnityEngine;

/// <summary>
/// Provides the functionality for the player to pickup the spanner
/// </summary>
public class SpannerInteractable : InventoryInteractableItem
{
    /// <summary>
    /// Implements the OnInteractedByPlayer function from IPlayerInteractable
    /// Called when the player presses the interact key while looking at this object
    /// </summary>
    public override void OnInteractedByPlayer()
    {
        base.OnInteractedByPlayer();
    }
}
