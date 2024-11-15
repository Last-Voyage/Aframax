/******************************************************************************
// File Name:       ReloadingUI.cs
// Author:          Ryan Swanson
// Creation Date:   October 28, 2024
//
// Description:     A temporary script that exists to show the ui for reloading
//                  Functionality will likely be moved to WeaponUI once that's committed
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Temporary script holding the reloading ui until the WeaponUI script holding the reticle functionality is in
/// </summary>
public class ReloadingUI : MonoBehaviour
{
    private TMP_Text _reloadingText;

    // Start is called before the first frame update
    void Start()
    {
        SetUpReloadingText();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    /// <summary>
    /// Sets up any needed functionality for the reloading text
    /// </summary>
    private void SetUpReloadingText()
    {
        _reloadingText = GetComponent<TMP_Text>();
        HideReloadingUI();
    }

    /// <summary>
    /// Shows the reloading UI
    /// </summary>
    private void ShowReloadingUI()
    {
        _reloadingText.enabled = true;
    }

    /// <summary>
    /// Hides the reloading UI
    /// </summary>
    private void HideReloadingUI()
    {
        _reloadingText.enabled = false;
    }

    /// <summary>
    /// Subscribes to any needed events
    /// </summary>
    private void SubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().AddListener(ShowReloadingUI);
        PlayerManager.Instance.GetOnHarpoonReloadedEvent().AddListener(HideReloadingUI);
    }

    /// <summary>
    /// Unsubscribes from any events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        PlayerManager.Instance.GetOnHarpoonFiredEvent().RemoveListener(ShowReloadingUI);
        PlayerManager.Instance.GetOnHarpoonReloadedEvent().RemoveListener(HideReloadingUI);
    }
}
