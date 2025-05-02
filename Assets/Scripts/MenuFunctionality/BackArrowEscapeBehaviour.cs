/*****************************************************************************
// File Name :         BackArrowEscapeBehaviour.cs
// Author :            Jeremiah Peters
// Contributor:        Nick Rice
// Creation Date :     11/18/24
//
// Brief Description : attached to the back arrow inside submenus, adds the functionality to press escape to go back
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Places the back button this is attached to the back button stack in the UI manager
/// </summary>
public class BackArrowEscapeBehaviour : MonoBehaviour
{
    /// <summary>
    /// Enables player input and swaps ui if needed
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.AddToBackStack(GetComponent<Button>());
    }

    /// <summary>
    /// Selects the previously selected UI
    /// </summary>
    public void SelectPreviousUi()
    {
        UiManager.Instance.SelectUiOnPreviousPage();
    }
}
