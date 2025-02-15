/**********************************************************************************************************************
// File Name :         AmmoRackInteractable.cs
// Author :            Andrew Stapay
// Contributors:       Jeremiah Peters
// Creation Date :     11/13/2024
//
// Brief Description : Implements an ammo rack where the player can refill their ammo.
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class that implements the interaction with the ammo rack
/// </summary>
public class AmmoRackInteractable : MonoBehaviour, IPlayerInteractable
{
    // The nuumber of harpoons that are currently on the rack
    private int _currentHarpoons;

    [SerializeField] public UnityEvent OnAmmoDepletedEvent;

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
        PlayerManager.Instance.OnInvokeHarpoonRestockEvent(this);
        if (_currentHarpoons == 0)
        {
            //disable interact prompt
            OnAmmoDepletedEvent?.Invoke();
        }
    }

    /// <summary>
    /// Removes a number of harpoons from the ammo rack
    /// </summary>
    /// <param name="numHarpoons"> the number of harpoons to remove </param>
    public void RemoveHarpoons(int numHarpoons)
    {
        for (int i = 0; i < numHarpoons; i++)
        {
            _currentHarpoons--;
            DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Gets the number of harpoons in this ammo rack
    /// </summary>
    /// <returns> The number of harpoons in the ammo rack </returns>
    public int GetNumHarpoons() => _currentHarpoons;
}
