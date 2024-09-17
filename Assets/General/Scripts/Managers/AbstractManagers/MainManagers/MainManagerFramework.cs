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
    public abstract void SetupMainManager();
}
