/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Nick Rice
// Creation Date :     3/1/25
//
// Brief Description : Selects UI buttons when the UI opens
*****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Makes UI buttons selected when a new UI screen pops up from gameplay
/// </summary>
public class InGameMenuSwap : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelectedButton;
    
    /// <summary>
    /// Makes UI buttons selected when a new UI screen pops up from gameplay
    /// </summary>
    private void OnEnable()
    {
        // EventSystem.current uses the event system that is in use
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
}
