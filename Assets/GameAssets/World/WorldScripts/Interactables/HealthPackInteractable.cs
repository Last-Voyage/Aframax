/*****************************************************************************
// File Name :         HealthPackInteractable.cs
// Author :            Andrew Stapay
// Creation Date :     11/12/24
//
// Brief Description : Controls an interactable health pack in scene. When
                       interacted with, it restores the player's health.
*****************************************************************************/
using UnityEngine;

/// <summary>
/// The class that contains the necessary methods to control the health pack
/// </summary>
public class HealthPackInteractable : MonoBehaviour, IPlayerInteractable
{
    // Variables to store the amount of health restored
    // and the number of uses before this object is destroyed
    [SerializeField] private int _healthRestored = 20;
    [SerializeField] private int _numUses = 3;

    /// <summary>
    /// An inherited method that triggers when the object is interacted with.
    /// Heals the player for a certain amount of health
    /// </summary>
    public void OnInteractedByPlayer()
    {
        PlayerManager.Instance.InvokePlayerHealEvent(_healthRestored);
        _numUses--;

        if (_numUses == 0)
        {
            Destroy(gameObject);
        }
    }
}
