/******************************************************************************
// File Name:       MainUniversalManagerFramework.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the framework to be used by all main universal managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for the main universal managers
/// Since Universal and Gameplay managers are pretty similar at the moment there isn't much here
/// </summary>
public class MainUniversalManagerFramework : MainManagerFramework
{
    /// <summary>
    /// Overrides the set up instance function
    /// </summary>
    public override void SetUpInstance()
    {

    }

    /// <summary>
    /// Subscribes the manager to needed events
    /// </summary>
    public override void SetupMainManager()
    {
        SubscribeToEvents();
    }
    
    /// <summary>
    /// Subscribes to universal events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.AddListener(SubscribeToGameplayEvents);
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.AddListener(UnsubscribeToGameplayEvents);
    }

    /// <summary>
    /// Unsubscribes to universal events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.RemoveListener(SubscribeToGameplayEvents);
        AframaxSceneManager.Instance.GetOnBeforeSceneChanged.RemoveListener(UnsubscribeToGameplayEvents);
    }

    /// <summary>
    /// Called after a gameplay scene is loaded
    /// Used to subscribe universal managers to gameplay manager events
    /// Since gameplay managers only exist in gameplay scenes you can't use the 
    ///     regular SubscribeToEvents as that is run when the universal managers are created
    /// </summary>
    protected virtual void SubscribeToGameplayEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribed to all gameplay events
    /// </summary>
    protected virtual void UnsubscribeToGameplayEvents()
    {
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
