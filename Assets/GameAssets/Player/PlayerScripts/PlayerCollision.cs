/*****************************************************************************
// File Name :         PlayerCollision.cs
// Author :            Ryan Swanson
// Contributor:        Andrea Swihart-DeCoster
// Creation Date :     10/16/24
//
// Brief Description : Controls the functionality for collisions
*****************************************************************************/

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
    /// <param name="contact"> The collider we just hit</param>
    private void OnTriggerEnter(Collider contact)
    {
        CheckForKillBoxContact(contact.gameObject);

        CheckForEnemyContact(contact.gameObject);

        CheckForStartVineChaseTrigger(contact);

        CheckForChaseDamageTrigger(contact);

        CheckForMusicTrigger(contact);

        CheckForSavePointTrigger(contact);
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
            PlayerManager.Instance.OnInvokePlayerDeath();
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

    /// <summary>
    /// Checks for trigger to activate chase sequence
    /// </summary>
    /// <param name="contact"></param>
    private void CheckForStartVineChaseTrigger(Collider contact)
    {
        if(contact.CompareTag("ChaseTrigger"))
        {
            ChaseVineGroup chaseVineGroup = contact.GetComponent<ChaseVineGroup>();
            if(chaseVineGroup != null && chaseVineGroup.IsTriggeredByPlayerWalkThrough())
            {
                chaseVineGroup.ActivateThisGroupOfVines();
            }
        }
    }

    /// <summary>
    /// Checks for the trigger to damage player in chase sequence
    /// </summary>
    /// <param name="contact">The collider we contacted</param>
    private void CheckForChaseDamageTrigger(Collider contact)
    {
        if(contact.CompareTag("ChaseDamageTrigger"))
        {
            ChaseVineGroup chaseVineGroup = contact.GetComponentInParent<ChaseVineGroup>();
            if(chaseVineGroup != null)
            {
                if(chaseVineGroup.IsSupposedToKillInstant())
                {
                    PlayerManager.Instance.OnInvokePlayerDeath();
                }
                else
                {
                    PlayerFunctionalityCore.Instance.GetPlayerHealth().TakeDamage(chaseVineGroup.GetPlayerDamageAmount(), null);
                }
            }
        }
    }

    /// <summary>
    /// Checks for the trigger to change the music
    /// </summary>
    /// <param name="contact">The collider we contacted</param>
    private void CheckForMusicTrigger(Collider contact)
    {
        if(contact.gameObject.TryGetComponent(out MusicSwapPlayerTrigger musicSwapPlayerTrigger))
        {
            musicSwapPlayerTrigger.PlayerContact();
        }
    }
    
    /// <summary>
    /// Checks for the trigger to save the game
    /// </summary>
    /// <param name="contact">The collider we contacted</param>
    private void CheckForSavePointTrigger(Collider contact)
    {
        if (contact.gameObject.TryGetComponent(out SavePointTrigger savePlayerTrigger))
        {
            savePlayerTrigger.PlayerContact();
        }
    }
    #endregion
}
