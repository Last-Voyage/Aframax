/*****************************************************************************
// File Name :         HudInventoryManager.cs
// Author :            Jeremiah Peters
// Creation Date :     3/2/25
//
// Brief Description : handles the player hud inventory icons for held items
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// used for enabling and disabling held item hud
/// </summary>
public class HudInventoryManager : MonoBehaviour
{
    [SerializeField] private Image[] _hudInventoryIcons;

    /// <summary>
    /// enables a held item icon
    /// </summary>
    public void SetInventoryImage(int desiredIconIndex)
    {
        DisableInventoryImages();
        _hudInventoryIcons[desiredIconIndex].gameObject.SetActive(true);
    }

    /// <summary>
    /// disables all held item icons
    /// </summary>
    public void DisableInventoryImages()
    {
        foreach (Image icons in _hudInventoryIcons)
        {
            icons.gameObject.SetActive(false);
        }
    }
}
