/******************************************************************************
// File Name:       InventoryInteractableTrigger.cs
// Author:          Charlie Polonus
// Creation Date:   February 3rd, 2025
//
// Description:     Contains the functionality for any interactable object
                    that does something if the player's inventory contains a
                    certain item
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The MonoBehaviour that manages objects that require certain inventory items
/// </summary>
public class InventoryInteractableTrigger : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private bool _canBeInteractedWith;
    [SerializeField] private List<string> _requiredItems = new();
    [SerializeField] private UnityEvent _onTriggerEvent;

    /// <summary>
    /// A virtual method for interacting with any object that requires an item to be in the inventory
    /// </summary>
    public virtual void OnSoundChange()
    {
        // Edge case: If the object can't be interacted with or player doesn't have all the items
        if (!_canBeInteractedWith
            || !PlayerInventory.Instance.HasItems(_requiredItems))
        {
            return;
        }

        // Successfully run the method
        _onTriggerEvent?.Invoke();
    }

    /// <summary>
    /// Add an item to the required item list
    /// </summary>
    /// <param name="itemName">The name of the item to add to the list</param>
    public void AddItem(string itemName)
    {
        if (!_requiredItems.Contains(itemName))
        {
            _requiredItems.Add(itemName);
        }
    }

    /// <summary>
    /// Sets whether or not the object can be interacted with
    /// </summary>
    /// <param name="value"></param>
    public void SetCanBeInteractedWith(bool value)
    {
        _canBeInteractedWith = value;
    }
}
