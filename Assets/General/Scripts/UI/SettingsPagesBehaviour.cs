/******************************************************************************
// File Name:       SettingsPagesBehaviour.cs
// Author:          Jeremiah Peters
// Creation Date:   February 24, 2025
//
// Description:     used to change the page of settings
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class comment placeholder
/// </summary>
public class SettingsPagesBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _settingsPages = new GameObject[3];

    public Button thetestbuttonname;

    public void SwitchSettingsPage(int pageToEnable)
    {
        //switch one on and the others off
        foreach(GameObject page in _settingsPages)
        {
            page.SetActive(false);
        }
        _settingsPages[pageToEnable].SetActive(true);

        //Button.
        //Debug.Log(thetestbuttonname.spriteState.pressedSprite);
        //Debug.Log(thetestbuttonname.spriteState = thetestbuttonname.spriteState.pressedSprite);
    }
}
