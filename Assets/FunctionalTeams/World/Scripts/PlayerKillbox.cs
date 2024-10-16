/*****************************************************************************
// File Name :         PlayerKillbox.cs
// Author :            Ryan Swanson
// Creation Date :     10/15/2024
//
// Brief Description : Killbox for killing the player out of bounds
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kills the player when out of bounds
/// </summary>
public class PlayerKillbox : MonoBehaviour
{
    /*
     * This is connected the old health as the new one isn't in yet.
     * Personally I would like to centralize player collisions more universally, but for now this gets the job done
     */

    private void OnTriggerEnter(Collider other)
    {
        //As mention above I would like a more centralized system, so we don't need to check for component
        PlayerHealthManager playerHealthManager = other.GetComponentInParent<PlayerHealthManager>();
        //Forcibly kills the player
        playerHealthManager?.PlayerDied();
    }
}
