/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Nick Rice
// Creation Date :     3/1/25
//
// Brief Description : Selects UI buttons when the UI opens
*****************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameMenuSwap : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelectedButton;
    
    private EventSystem _eventSystem;
    
    private void OnEnable()
    {
        _eventSystem = FindObjectOfType<EventSystem>();
        _eventSystem.SetSelectedGameObject(_firstSelectedButton);
    }
}
