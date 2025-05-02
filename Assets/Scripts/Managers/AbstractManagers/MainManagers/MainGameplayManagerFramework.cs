/******************************************************************************
// File Name:       MainGameplayManagerFramework.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the framework to be used by all main gameplay managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for the main gameplay managers
/// Since Universal and Gameplay managers are pretty similar at the moment there isn't much here
/// </summary>
public class MainGameplayManagerFramework : MainManagerFramework
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
    public override void SetUpMainManager()
    {
        SubscribeToEvents();
    }

    /// <summary>
    /// Used to subscribe to all events required for functionality
    /// </summary>
    protected override void SubscribeToEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribes from all events on destruction
    /// </summary>
    protected override void UnsubscribeToEvents()
    {

    }
}
