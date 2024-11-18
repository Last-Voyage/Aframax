using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableUI : MonoBehaviour
{
    private GameObject _interactUI;

    /// <summary>
    /// Right away finds the InteractionUI object for future use and disables it before player can see
    /// </summary>
    private void Awake()
    {
        _interactUI = GameObject.Find("InteractionUI");
        _interactUI.SetActive(false);
    }

    /// <summary>
    /// If the active status of _interactUI does not match what it should, it sets the status to match the case
    /// </summary>
    /// <param name="uiToggle">case for whether or not _interactUI should be active</param>
    public void SetInteractUIStatus(bool uiToggle)
    {
        if (uiToggle != _interactUI.activeInHierarchy) { 
            _interactUI.SetActive(uiToggle);
        }
    }
}
