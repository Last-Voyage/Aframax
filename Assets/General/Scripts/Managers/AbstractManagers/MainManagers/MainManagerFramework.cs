/******************************************************************************
// File Name:       MainManagerFramework.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the framework to be used by all main managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the framework used across ALL main managers
/// Both universal and gameplay
/// </summary>
public abstract class MainManagerFramework : MonoBehaviour
{
    /// <summary>
    /// Establishes the instance relating to the manager
    /// </summary>
    public abstract void SetupInstance();
    /// <summary>
    /// Performs any needed setup specific to the manager
    /// </summary>
    public abstract void SetupMainManager();
    /// <summary>
    /// Used to subscribe to all events required for functionality
    /// </summary>
    protected abstract void SubscribeToEvents();
    /// <summary>
    /// Unsubscribes from all events on destruction
    /// </summary>
    protected abstract void UnsubscribeToEvents();
    protected virtual void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
