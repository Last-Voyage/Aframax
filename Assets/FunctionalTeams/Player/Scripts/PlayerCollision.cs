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
        CheckForKillboxContact(other);
    }

    /// <summary>
    /// Checks for if you are staying in trigger contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        
    }

    /// <summary>
    /// Checks for the end of trigger contact
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        
    }
    #endregion

    #region Collision Contact
    /// <summary>
    /// Checks for the start of collision contact
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        CheckForEnemyContact(collision);
    }

    /// <summary>
    /// Checks for if you are staying in collision contact
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay(Collision collision)
    {
        
    }

    /// <summary>
    /// Check for the end of collision contact
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        
    }
    #endregion

    #region Contact Checks
    /// <summary>
    /// Checks if hte player hit a killbox
    /// </summary>
    /// <param name="other"></param>
    private void CheckForKillboxContact(Collider other)
    {
        print("Check");
        if(other.CompareTag(KILLBOX_TAG))
        {
            print("Killbox");
            PlayerManager.Instance.InvokeOnPlayerDeath();
        }
    }

    /// <summary>
    /// Checks for if the player makes contact with an enemy
    /// </summary>
    /// <param name="collision"></param>
    private void CheckForEnemyContact(Collision collision)
    {
        //Currently using this until Mark is done with his universal damage system
        //TEMPORARY!
        EnemyDamageTemp enemyDamageTemp = collision.gameObject.GetComponent<EnemyDamageTemp>();
        if(enemyDamageTemp!=null)
        {
            //Yeah I know this is inefficient, but its temporary. Let me have this
            print(enemyDamageTemp.AttackPower);
            GetComponentInParent<PlayerHealth>().TempEnemyDamage(enemyDamageTemp.AttackPower);
        }
        
    }
    #endregion
}
