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
public class HealthPackInteractable : TogglableInteractable, IPlayerInteractable
{
    // Variables to store the amount of health restored
    // and the number of uses before this object is destroyed
    [SerializeField] private int _healthRestored = 20;
    [SerializeField] private int _numUses = 3;

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateInteractability);
    }

    /// <summary>
    /// Unsubscribes to any subscribed events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().RemoveListener(UpdateInteractability);
    }

    /// <summary>
    /// An inherited method that triggers when the object is interacted with.
    /// Heals the player for a certain amount of health
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if(!_canInteract)
        {
            return;
        }

        PlayerManager.Instance.OnInvokePlayerHealEvent(_healthRestored);
        _numUses--;

        if (_numUses == 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the interactability of the associated world space popup
    /// </summary>
    /// <param name="percentHealth"> The percent health variable from the event </param>
    /// <param name="currentHealth"> The current health variable from the event </param>
    private void UpdateInteractability(float percentHealth, float currentHealth)
    {
        _canInteract = percentHealth < 1;
        UpdateInteractablePopupToggle();
    }
}
