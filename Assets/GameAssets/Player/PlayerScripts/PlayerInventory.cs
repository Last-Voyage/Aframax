/**********************************************************************************************************************
// File Name :         PlayerInventory.cs
// Author :            Ryan Swanson
// Contributer :       Charlie Polonus, Nick Rice
// Creation Date :     11/12/24
// 
// Brief Description : Stores any items held by the player
**********************************************************************************************************************/

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Stores items held by the player
/// Done here in order to have a centralized place for any pickups
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [SerializeField] private List<string> _allItems = new();

    /// <summary>
    /// Initializes the singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Adds an item to the player's inventory
    /// </summary>
    /// <param name="itemName">The name of the item being added</param>
    public void AddItem(string itemName)
    {
        // Add the item if the player doesn't already have it
        if (!_allItems.Contains(itemName.ToLower()))
        {
            _allItems.Add(itemName.ToLower());
        }
    }

    /// <summary>
    /// Checks to see if the player has a certain item
    /// </summary>
    /// <param name="itemName">The name of the item being checked</param>
    /// <returns>Whether or not the player has that specific item</returns>
    public bool HasItem(string itemName)
    {
        return _allItems.Contains(itemName.ToLower());
    }

    /// <summary>
    /// Checks to see if the player has all items in a list
    /// </summary>
    /// <param name="itemNames">The names of the items being checked</param>
    /// <returns>Whether or not the player has all the specific items</returns>
    public bool HasItems(IEnumerable<string> itemNames)
    {
        // Edge case: if the list is null return true
        if (itemNames == null)
        {
            return true;
        }

        // Check to see if the player is missing any item, if they are then return false
        foreach (string curItem in itemNames)
        {
            if (!_allItems.Contains(curItem.ToLower()))
            {
                return false;
            }
        }

        // Succesfully return that the player has the item
        return true;
    }

    /// <summary>
    /// Saves the players inventory
    /// </summary>
    private void SaveInventory()
    {
        SaveManager.Instance.GetGameSaveData().SetPlayerInventory(_allItems);
    }

    /// <summary>
    /// Loads in the players inventory from saved data
    /// </summary>
    private void LoadInventory()
    {
        _allItems = SaveManager.Instance.GetGameSaveData().GetCurrentInventory();
    }

    /// <summary>
    /// Adds listeners for saving and loading data
    /// </summary>
    private void OnEnable()
    {
        SaveManager.Instance.GetOnNewCheckpoint()?.AddListener(SaveInventory);
        SaveManager.Instance.GetOnLoadSaveData()?.AddListener(LoadInventory);
    }

    /// <summary>
    /// Removes listeners for saving and loading data
    /// </summary>
    private void OnDisable()
    {
        SaveManager.Instance.GetOnNewCheckpoint()?.RemoveListener(SaveInventory);
        SaveManager.Instance.GetOnLoadSaveData()?.RemoveListener(LoadInventory);
    }
}
