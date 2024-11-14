/**********************************************************************************************************************
// File Name :         AmmoRackInteractable.cs
// Author :            Andrew Stapay
// Creation Date :     11/13/2024
//
// Brief Description : Controls the UI that displays information about the player's ammo count
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the UI for the ammo count
/// </summary>
public class AmmoUI : MonoBehaviour
{
    private TMP_Text _ammoText;

    /// <summary>
    /// Called when the game starts
    /// Used to get the text and subscribe to events
    /// </summary>
    private void Awake()
    {
        SetUpText();
        SubscribeToEvents();
    }

    /// <summary>
    /// Called when this game object is destroyed
    /// Used to unsubscribe to events
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Get the text component of this UI element
    /// </summary>
    private void SetUpText()
    {
        _ammoText = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Add listeners to the necessary events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(ReduceMainAmmo);
        PlayerManager.Instance.GetOnHarpoonReloadedEvent().AddListener(RestoreMainAmmo);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(RestoreReserveAmmo);
    }

    /// <summary>
    /// Remove listeners of the necessary events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(ReduceMainAmmo);
        PlayerManager.Instance.GetOnHarpoonReloadedEvent().RemoveListener(RestoreMainAmmo);
        PlayerManager.Instance.GetOnHarpoonRestockCompleteEvent().AddListener(RestoreReserveAmmo);
    }

    /// <summary>
    /// Updates the UI when the harpoon is fired
    /// </summary>
    private void ReduceMainAmmo()
    {
        _ammoText.text = "0/" + _ammoText.text.Substring(2);
    }

    /// <summary>
    /// Updates the UI when the harpoon is reloaded
    /// </summary>
    private void RestoreMainAmmo()
    {
        _ammoText.text = "1/" + (int.Parse(_ammoText.text.Substring(2)) - 1);
    }

    /// <summary>
    /// Updates the UI when the player restocks on ammo
    /// </summary>
    /// <param name="unused"> An unused reference to the ammo rack used </param>
    private void RestoreReserveAmmo(int unused)
    {
        _ammoText.text = _ammoText.text.Substring(0, 2) + HarpoonGun.Instance.GetReserveAmmo();
    }
}
