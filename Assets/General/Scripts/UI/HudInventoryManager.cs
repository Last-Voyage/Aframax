/*****************************************************************************
// File Name :         HudInventoryManager.cs
// Author :            Jeremiah Peters
// Creation Date :     3/2/25
//
// Brief Description : 
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudInventoryManager : MonoBehaviour
{
    [SerializeField] private Image[] _hudInventoryIcons;

    /// <summary>
    /// 
    /// </summary>
    public void setInventoryImage(int desiredIconIndex)
    {
        disableInventoryImages();
        _hudInventoryIcons[desiredIconIndex].gameObject.SetActive(true);
    }

    public void disableInventoryImages()
    {
        foreach (Image icons in _hudInventoryIcons)
        {
            icons.gameObject.SetActive(false);
        }
    }
}
