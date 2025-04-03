/**********************************************************************************************************************
// File Name :          TriggerDatamoshingEffect.cs
// Author :             Miles Rogers
// Creation Date :      4/1/2025
//
// Brief description :  Triggers the datamoshing effect for a set amount of seconds. References the Player camera's
//                      DatamoshEffectManager to ensure the Coroutine plays until the end if the object gets
//                      destroyed.
**********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Volume trigger that plays the datamoshing effect for a set amount of seconds
/// </summary>
public class TriggerDatamoshingEffect : MonoBehaviour
{
    /// <summary>
    /// How long to play the datamoshing effect for when the player
    /// enters the object's BoxCollider trigger
    /// </summary>
    [SerializeField] private float _effectDuration = 1.0F;
    
    /// <summary>
    /// Reference to the global datamosh effect manager (so Coroutines
    /// are unaffected by objects being destroyed)
    /// </summary>
    private DatamoshEffectManager _datamoshEffectManager;

    /// <summary>
    /// Grab instance of the DatamoshEffectManager singleton
    /// </summary>
    private void Start()
    {
        _datamoshEffectManager = DatamoshEffectManager.Instance;
    }

    /// <summary>
    /// Trigger the datamoshing effect when the player enters
    /// the object's BoxCollider trigger
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            _datamoshEffectManager.PlayDatamoshEffect(_effectDuration);
        }
    }
}
