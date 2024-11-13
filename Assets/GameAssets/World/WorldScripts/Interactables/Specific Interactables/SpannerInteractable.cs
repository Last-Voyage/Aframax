/******************************************************************************
// File Name:       SpannerInteractable.cs
// Author:          Ryan Swanson
// Creation Date:   October 12th, 2024
//
// Description:     Contains the functionality for the spanner interactable
******************************************************************************/

using UnityEngine;

public class SpannerInteractable : MonoBehaviour, IPlayerInteractable
{
    /// <summary>
    /// Implements the OnInteractedByPlayer function from IPlayerInteractable
    /// Called when the player presses the interact key while looking at this object
    /// </summary>
    void IPlayerInteractable.OnInteractedByPlayer()
    {
        PlayerInventory.Instance.DoesPlayerHaveSpanner = true;
        //I imagine there will be an animation or vfx for this in the future, but for now we just destroy it
        Destroy(gameObject);
    }
}
