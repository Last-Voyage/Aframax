/******************************************************************************
// File Name:       MainManagerFramework.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the framework to be used by the core managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework to the 2 core managers
/// </summary>
public abstract class CoreManagersFramework : MonoBehaviour
{
    /// <summary>
    /// Tries to instance the manager, if it fails it destroys itself :(
    /// </summary>
    protected virtual void Awake()
    {
        //Attempts to establish the instance
        if(EstablishInstance())
        {
            //If successful setup all main managers
            SetupMainManagers();
        }
        else
        {
            //If it fails, it destroys itself
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Attempts to establish the singleton
    /// Returns if it succeeded
    /// </summary>
    /// <returns></returns>
    protected abstract bool EstablishInstance();

    /// <summary>
    /// Sets up all managers that can be accessed from this one
    /// </summary>
    protected abstract void SetupMainManagers();
}
