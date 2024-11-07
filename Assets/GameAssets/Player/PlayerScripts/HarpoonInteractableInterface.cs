/**********************************************************************************************************************
// File Name :         PlayerInteractableInterface.cs
// Author :            Alex Kalscheur
// Creation Date :     11/7/2024
//
// Brief Description : Interface for implementing interactable system with harpoon
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for implementing interactable system with player
/// </summary>
public interface HarpoonInteractableInterface
{
    /// <summary>
    /// Will be called when the object is hit by a harpoon
    /// </summary>
    public void OnHitByHarpoon();
}
