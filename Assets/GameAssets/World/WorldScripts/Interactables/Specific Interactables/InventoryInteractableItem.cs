/******************************************************************************
// File Name:       InventoryInteractableItem.cs
// Author:          Charlie Polonus
// Creation Date:   February 3rd, 2025
//
// Description:     Contains the functionality for any interactable item that
                    can be added to the inventory
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The MonoBehaviour handling items being added to the players inventory
/// </summary>
public class InventoryInteractableItem : MonoBehaviour, IPlayerInteractable
{
    [SerializeField] private string _itemName;
    [SerializeField] private bool _destoryOnPickup;
    [SerializeField] private UnityEvent _pickupEvent;

    /// <summary>
    /// A virtual method for picking up an item to be added to the inventory
    /// </summary>
    public virtual void OnInteractedByPlayer()
    {
        // Add the item to the player's inventory
        PlayerInventory.Instance.AddItem(_itemName);

        // Run the pickup events
        _pickupEvent?.Invoke();

        // Destroy the object if it needs to be removed
        if (_destoryOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
