/**********************************************************************************************************************
// File Name :         PlayerInteractibleInterface.cs
// Author :            Alex Kalscheur
// Creation Date :     11/7/2024
//
// Brief Description : Interface for implementing interactible system with player
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for implementing interactible system with player
/// </summary>
public interface PlayerInteractibleInterface
{
    /// <summary>
    /// Will be called when interacted with by the player
    /// </summary>
    public void OnInteractedByPlayer();
}
