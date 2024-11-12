/**********************************************************************************************************************
// File Name :         PlayerInventory.cs
// Author :            Ryan Swanson
// Creation Date :     11/12/24
// 
// Brief Description : Stores any items held by the player
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores items held by the player
/// Done here in order to have a centralized place for any pickups
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public bool DoesPlayerHaveSpanner { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EstablishInstance();
    }

    /// <summary>
    /// Creates the instance of the inventory
    /// </summary>
    private void EstablishInstance()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
