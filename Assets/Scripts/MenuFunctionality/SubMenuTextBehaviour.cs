/*****************************************************************************
// File Name :         SubMenuTextBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     11/18/24
//
// Brief Description : provides button functionality for enabling and disabling UI elements
                        used for navigating submenus that are not distinct scenes
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// button functionality for disabling and enabling ui elements, as well as disabling and enabling unpausing
/// </summary>
public class SubMenuTextBehaviour : MonoBehaviour
{
    [SerializeField] private List<GameObject> _menuElements;

    /// <summary>
    /// button functionality for turning on ui elements
    /// </summary>
    /// <param name="menuElementListID"></param>
    public void EnableMenuElement(int menuElementListID)
    {
        _menuElements[menuElementListID].SetActive(true);
    }

    /// <summary>
    /// button functionality for turning off ui elements
    /// </summary>
    /// <param name="menuElementListID"></param>
    public void DisableMenuElement(int menuElementListID)
    {
        _menuElements[menuElementListID].SetActive(false);
    }

    /// <summary>
    /// changes the player's ability to pause/unpause, used to disable it during certain menus
    /// </summary>
    public void SetPauseToggle(bool canPause)
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(canPause);
    }
}
