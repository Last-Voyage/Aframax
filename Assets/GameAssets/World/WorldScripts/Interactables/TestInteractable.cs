/**********************************************************************************************************************
// File Name :         TestInteractable.cs
// Author :            Alex Kalscheur
// Creation Date :     11/13/24
// 
// Brief Description : Simple test script for use in InteractableTestGym
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// class for interactable test
/// </summary>
public class TestInteractable : MonoBehaviour, IPlayerInteractable
{
    /// <summary>
    /// When interacted upon, sends a silly little console message and destroys the object
    /// </summary>
    public void OnInteractedByPlayer()
    {
        Debug.Log("You killed me :(");
        Destroy(gameObject);
    }
}
