/*****************************************************************************
// File Name :         PlayerCollision.cs
// Author :            Ryan Swanson
// Contributor:        Andrea Swihart-DeCoster
// Creation Date :     10/16/24
//
// Brief Description : Controls the functionality for collisions
*****************************************************************************/

using Unity.VisualScripting;
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

        CheckForEnemyContact(contact);

        CheckForWallCeilingEnemyTriggerContact(contact);

        CheckForStartVineChaseTrigger(contact);

        CheckForChaseDamageTrigger(contact);

        CheckForMusicTrigger(contact, true);

        CheckForSavePointTrigger(contact);

        CheckForAppearTrigger(contact);

        CheckForWhackAMoleTrigger(contact);

        CheckForGeneralTrigger(contact);
    }

    /// <summary>
    /// Checks for the end of trigger contact
    /// </summary>
    /// <param name="contact">The collider we just left </param>
    private void OnTriggerExit(Collider contact)
    {
        CheckForMusicTrigger(contact, false);
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
    /// Checks for if the player makes contact with an enemy and damages player
    /// </summary>
    /// <param name="collision"> The object that we are checking for if it is an enemy </param>
    private void CheckForEnemyContact(Collider contact)
    {
        if(contact.CompareTag("Enemy"))
        {
            WallCeilingAttack attackScript = contact.GetComponentInParent<WallCeilingAttack>(contact);

            if(!attackScript.IsUnityNull()) 
            {
                attackScript.DamagePlayer();
            }
        }
    }

    /// <summary>
    /// Checks for if the player makes contact with an enemy trigger
    /// </summary>
    /// <param name="collision"> The object that we are checking for if it is an enemy </param>
    private void CheckForWallCeilingEnemyTriggerContact(Collider contact)
    {
        if (contact.CompareTag("WallCeilingTrigger"))
        {
            WallCeilingAttack attackScript = contact.GetComponentInParent<WallCeilingAttack>(contact);

            if (!attackScript.IsUnityNull())
            {
                attackScript.ActivateAttack();
            }
        }
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
            if(!chaseVineGroup.IsUnityNull() && chaseVineGroup.IsTriggeredByPlayerWalkThrough())
            {
                StartCoroutine(chaseVineGroup.ActivateThisGroupOfVines());
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
            if(!chaseVineGroup.IsUnityNull())
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
    /// Checks for the trigger to damage player in chase sequence
    /// </summary>
    /// <param name="contact">The collider we contacted</param>
    private void CheckForWhackAMoleTrigger(Collider contact)
    {
        if (contact.CompareTag("WhackAMoleTrigger"))
        {
            var whackAMoleObject = contact.transform.parent.GetComponent<WhackAMole>();

            whackAMoleObject.CallAttack(transform);
        }
    }

    /// <summary>
    /// Checks for the trigger to change the music
    /// </summary>
    /// <param name="contact">The collider we contacted</param>
    /// <param name="isEnter">If the collision came from entering</param>
    private void CheckForMusicTrigger(Collider contact, bool isEnter)
    {
        if(contact.gameObject.TryGetComponent(out MusicSwapPlayerTrigger musicSwapPlayerTrigger))
        {
            if(isEnter)
            {
                musicSwapPlayerTrigger.PlayerContact();
            }
            else
            {
                musicSwapPlayerTrigger.PlayerExit();
            }
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
            SaveManager.Instance.SaveText();
        }
    }

    private void CheckForAppearTrigger(Collider contact)
    {
        if(contact.CompareTag("AppearTrigger"))
        {
            //the component should always be on the 3rd child of the vine base
            ProceduralVine proceduralVine = contact.transform.parent.GetChild(2).GetComponent<ProceduralVine>();
            if (!proceduralVine.IsUnityNull())
            {
                if(proceduralVine.GetVineState() != ProceduralVine.EVineState.appearing && proceduralVine.GetVineState() != ProceduralVine.EVineState.shifting && !proceduralVine.GetIsAppeared())
                {
                    if(proceduralVine.IsWhackAMoleVine)
                    {
                        proceduralVine.StartAppear(transform);
                    }
                }
                
            }
        }
    }

    /// <summary>
    /// Check for a general trigger for contact
    /// </summary>
    /// <param name="contact">The object we contacted</param>
    private void CheckForGeneralTrigger(Collider contact)
    {
        if(contact.TryGetComponent<GeneralPlayerCollisionTrigger>
            (out  GeneralPlayerCollisionTrigger generalPlayerCollisionTrigger))
        {
            generalPlayerCollisionTrigger.PlayerContact();
        }
    }
    #endregion
}
