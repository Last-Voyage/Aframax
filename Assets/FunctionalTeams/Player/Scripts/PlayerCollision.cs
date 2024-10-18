/*****************************************************************************
// File Name :         PlayerCollision.cs
// Author :            Ryan Swanson
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
    private const string KILLBOX_TAG = "Killbox";

    #region Trigger Contact

    /// <summary>
    /// Checks for the start of trigger contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        CheckForKillboxContact(other.gameObject);

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
    /// Checks if hte player hit a killbox
    /// </summary>
    /// <param name="other"></param>
    private void CheckForKillboxContact(GameObject contact)
    {
        if(contact.CompareTag(KILLBOX_TAG))
        {
            PlayerManager.Instance.InvokeOnPlayerDeath();
        }
    }

    /// <summary>
    /// Checks for if the player makes contact with an enemy
    /// </summary>
    /// <param name="collision"></param>
    private void CheckForEnemyContact(GameObject contact)
    {
        //Currently using this until Mark is done with his universal damage system
        //TEMPORARY!
        EnemyDamageTemp enemyDamageTemp = contact.gameObject.GetComponentInChildren<EnemyDamageTemp>();
        if(enemyDamageTemp!=null)
        {
            //THIS IS TEMPORARY - Waiting for Marks Damage system which I imagine
            //will complete the needed functionality to connect damage to health
            //Delete this immediately after the universal damage system is implemented
            GetComponentInParent<PlayerHealth>().TempEnemyDamage(enemyDamageTemp.AttackPower);
        }
        
    }
    
    #endregion
}
