/*****************************************************************************
// File Name :         HealthPackInteractable.cs
// Author :            Andrew Stapay
// Contributors:     Mark Hanson
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
    public bool InteractEnabled { get; set; }
    
    private void Awake()
    {
        InteractEnabled = false;
        OnSubscribe();
    }

    /// <summary>
    /// Attach functions to events for Interaction being enabled or disabled
    /// </summary>
    public void OnSubscribe()
    {
        PlayerManager.Instance.GetOnPlayerHealEvent().AddListener(OnInteractTurnDisabled);
        PlayerManager.Instance.GetOnPlayerDamageEvent().AddListener(OnInteractTurnEnabled);
    }
    
/// <summary>
/// Taking off events after usage of them
/// </summary>
    public void OnUnsubscribe()
    {
        PlayerManager.Instance.GetOnPlayerHealEvent().RemoveListener(OnInteractTurnDisabled);
        PlayerManager.Instance.GetOnPlayerDamageEvent().RemoveListener(OnInteractTurnEnabled);
    }
    
    /// <summary>
    /// Function for enabling Interaction for health packs
    /// </summary>
    /// <param name="damage">damage variable to call event and nothing else</param>
    private void OnInteractTurnEnabled(float damage)
    {
        if (!InteractEnabled)
        {
            InteractEnabled = true;
        }
    }
/// <summary>
/// Function for disabling Interaction for health packs
/// </summary>
/// <param name="heal">heal variable to call event and nothing else</param>
    private void OnInteractTurnDisabled(float heal)
    {
        if (InteractEnabled)
        {
            InteractEnabled = false;
        }
    }
    
    
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
