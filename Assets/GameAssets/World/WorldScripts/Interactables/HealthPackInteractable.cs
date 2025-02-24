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

    private WorldSpacePopups _interactPopup;
    private bool _canInteract = false;

    /// <summary>
    /// Performs initial setup
    /// </summary>
    private void Start()
    {
        SubscribeToEvents();
        SetInitialVariables();
    }

    /// <summary>
    /// Cleans up anything after destruction
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Sets any variables to what they should be on start
    /// </summary>
    private void SetInitialVariables()
    {
        _interactPopup = GetComponentInChildren<WorldSpacePopups>();
        UpdateInteractablePopupToggle();
    }

    /// <summary>
    /// returns the number of uses
    /// </summary>
    /// <returns></returns>
    public int GetNumOfUses()
    {
        return _numUses;
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnPlayerHealthChangeEvent().AddListener(UpdateInteractability);
    }

    /// <summary>
    /// Unsubscribes to any subscribed events
    /// </summary>
    private void UnsubscribeToEvents()
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

    /// <summary>
    /// Updates the status of the interactable popup
    /// </summary>
    private void UpdateInteractablePopupToggle()
    {
        _interactPopup.TogglePopUp(_canInteract);
    }
}
