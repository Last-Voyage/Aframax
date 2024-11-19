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

    /// <summary>
    /// turns off the ability to toggle pausing, used to prevent unpausing from submenus
    /// </summary>
    public void DisablePauseToggle()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(true);
    }

    /// <summary>
    /// re-enables the ability to toggle pausing. make sure to do this back on if it is turned off!
    /// </summary>
    public void EnablePauseToggle()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(false);
    }
}
