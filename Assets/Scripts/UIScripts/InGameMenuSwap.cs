/*****************************************************************************
// File Name :         InGameMenuSwap.cs
// Author :            Nick Rice
// Contributors :      Adam Garwacki
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
        DeselectMenu();
    }

    /// <summary>
    /// Deselects the pause menu option currently active.
    /// </summary>
    public void DeselectMenu()
    {
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
}
