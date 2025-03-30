/*****************************************************************************
// File Name :         PlayerReticle.cs
// Author :            Ryan Swanson
// Creation Date :     2/10/25
//
// Brief Description : Provides a class that other interactables can inherit from to toggle interactability
*****************************************************************************/

using UnityEngine;

/// <summary>
/// Provides a class to toggle interactability
/// </summary>
public class TogglableInteractable : MonoBehaviour
{
    protected WorldSpacePopups _interactPopup;
    protected bool _canInteract = false;

    /// <summary>
    /// Performs initial setup
    /// </summary>
    protected virtual void Start()
    {
        SubscribeToEvents();
        SetInitialVariables();
    }

    /// <summary>
    /// Cleans up anything after destruction
    /// </summary>
    protected virtual void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Sets any variables to what they should be on start
    /// </summary>
    protected virtual void SetInitialVariables()
    {
        _interactPopup = GetComponentInChildren<WorldSpacePopups>();
        UpdateInteractablePopupToggle();
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    protected virtual void SubscribeToEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribes to any subscribed events
    /// </summary>
    protected virtual void UnsubscribeToEvents()
    {
        
    }

    /// <summary>
    /// Updates the status of the interactable popup
    /// </summary>
    protected virtual void UpdateInteractablePopupToggle()
    {
        _interactPopup.TogglePopUp(_canInteract);
    }
}
