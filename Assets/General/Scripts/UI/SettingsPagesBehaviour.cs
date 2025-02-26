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

    [SerializeField] private Button _audioButton;

    [SerializeField] private Button _videoButton;

    [SerializeField] private Button _gameplayButton;

    private ColorBlock _notFocusedColors;

    private ColorBlock _yesFocusedColors;

    private void Start()
    {
        _yesFocusedColors = _audioButton.colors;
        _notFocusedColors = _videoButton.colors;
    }

    public void SwitchSettingsPage(int pageToEnable)
    {
        //switch one on and the others off
        foreach(GameObject page in _settingsPages)
        {
            page.SetActive(false);
        }
        _settingsPages[pageToEnable].SetActive(true);

        switch (pageToEnable)
        {
            case 0:
                //audio
                _audioButton.colors = _yesFocusedColors;
                _videoButton.colors = _notFocusedColors;
                _gameplayButton.colors = _notFocusedColors;
                break;
            case 1:
                //video
                _audioButton.colors = _notFocusedColors;
                _videoButton.colors = _yesFocusedColors;
                _gameplayButton.colors = _notFocusedColors;
                break;
            case 2:
                //gameplay
                _audioButton.colors = _notFocusedColors;
                _videoButton.colors = _notFocusedColors;
                _gameplayButton.colors = _yesFocusedColors;
                break;
            default:
                //this should not happen
                break;
        }
    }
}
