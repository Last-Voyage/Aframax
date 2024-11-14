/**********************************************************************************************************************
// File Name :         AmmoRackInteractable.cs
// Author :            Andrew Stapay
// Creation Date :     11/13/2024
//
// Brief Description : Implements an ammo rack where the player can refill their ammo.
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that implements the interaction with the ammo rack
/// </summary>
public class AmmoRackInteractable : MonoBehaviour, IPlayerInteractable
{
    // The nuumber of harpoons that are currently on the rack
    private int _currentHarpoons;

    /// <summary>
    /// Called when the game starts
    /// Sets the number of harpoons to the number of game objects in the ammo rack in-engine
    /// </summary>
    private void Awake()
    {
        _currentHarpoons = transform.GetChild(0).childCount;
    }

    /// <summary>
    /// Triggers when the ammo rack is interacted with
    /// </summary>
    public void OnInteractedByPlayer()
    {
        PlayerManager.Instance.InvokeOnHarpoonRestockEvent(this);
    }

    /// <summary>
    /// Removes a harpoon from the ammo rack
    /// </summary>
    public void RemoveHarpoon()
    {
        _currentHarpoons--;
        DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
    }

    /// <summary>
    /// Determines if there are any more harpoons in the ammo rack
    /// </summary>
    /// <returns> True if there are no more harpoons in the rack, false otherwise. </returns>
    public bool OutOfHarpoons()
    {
        return _currentHarpoons == 0;
    }
}
