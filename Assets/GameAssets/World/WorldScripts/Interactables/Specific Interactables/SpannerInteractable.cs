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
    /// Called when the player presses the interact key on it
    /// </summary>
    protected void OnA()
    {
        
    }

    void IPlayerInteractable.OnInteractedByPlayer()
    {
        PlayerInventory.Instance.DoesPlayerHaveSpanner = true;
        //I imagine there will be an animation or vfx for this in the future, but for now we just destroy it
        Destroy(gameObject);
    }
}
