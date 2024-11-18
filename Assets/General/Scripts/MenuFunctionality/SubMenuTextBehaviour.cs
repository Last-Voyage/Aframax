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

public class SubMenuTextBehaviour : MonoBehaviour
{
    [SerializeField] private List<GameObject> MenuElements;

    /// <summary>
    /// button functionality for turning on ui elements
    /// </summary>
    /// <param name="MenuElementListID"></param>
    public void EnableMenuElement(int MenuElementListID)
    {
        MenuElements[MenuElementListID].SetActive(true);
    }

    /// <summary>
    /// button functionality for turning off ui elements
    /// </summary>
    /// <param name="MenuElementListID"></param>
    public void DisableMenuElement(int MenuElementListID)
    {
        MenuElements[MenuElementListID].SetActive(false);
    }

    public void DisablePauseToggle()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(true);
    }

    public void EnablePauseToggle()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(false);
    }
}
