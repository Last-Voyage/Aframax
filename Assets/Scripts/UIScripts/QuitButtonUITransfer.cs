/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Nick Rice
// Creation Date :     3/1/25
//
// Brief Description : Selects UI buttons when the quit UI closes
*****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Selects UI buttons when the quit UI closes
/// </summary>
public class QuitButtonUITransfer : MonoBehaviour
{
    private Button _firstSelectedButton;
    
    private EventSystem _eventSystem;
    
    private GameObject _noButton;

    /// <summary>
    /// Selects UI buttons when the quit UI closes
    /// </summary>
    private void OnDisable()
    {
        _eventSystem = FindObjectOfType<EventSystem>();

        _firstSelectedButton = FindObjectOfType<Button>();
        
        _eventSystem.SetSelectedGameObject(_firstSelectedButton.gameObject);
    }
}
