/*****************************************************************************
// File Name :         PlayerCollision.cs
// Author :            Ryan Swanson
// Contributor:        Andrea Swihart-DeCoster
// Creation Date :     10/16/24
//
// Brief Description : Controls the functionality for collisions
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the collision functionality
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    private const string _KILLBOX_TAG = "Killbox";

    #region Trigger Contact

    /// <summary>
    /// Checks for the start of trigger contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        CheckForKillBoxContact(other.gameObject);

        CheckForEnemyContact(other.gameObject);
    }

    #endregion

    #region Collision Contact
    /// <summary>
    /// Checks for the start of collision contact
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        CheckForEnemyContact(collision.gameObject);
    }

    #endregion

    #region Contact Checks
    
    /// <summary>
    /// Checks if the player hit a killbox
    /// </summary>
    /// <param name="other"> The object that we are checking for if it is a killbox </param>
    private void CheckForKillBoxContact(GameObject contact)
    {
        if(contact.CompareTag(_KILLBOX_TAG))
        {
            PlayerManager.Instance.InvokeOnPlayerDeath();
        }
    }

    /// <summary>
    /// Checks for if the player makes contact with an enemy
    /// </summary>
    /// <param name="collision"> The object that we are checking for if it is an enemy </param>
    private void CheckForEnemyContact(GameObject contact)
    {
        //TODO: Implement later
    }
    
    #endregion
}
