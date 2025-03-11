/******************************************************************************
// File Name:       SettingsPagesBehaviour.cs
// Author:          Jeremiah Peters
// Creation Date:   February 24, 2025
//
// Description:     used to change the page of settings and update buttons accordingly
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// operates setting pages and their respective buttons
/// </summary>
public class SettingsPagesBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] _settingsPages = new GameObject[3];

    [SerializeField] private Button _audioButton;

    [SerializeField] private Button _videoButton;

    [SerializeField] private Button _gameplayButton;

    [SerializeField] private Slider _topAudioSlider;

    [SerializeField] private Toggle _topGameplayToggle;
    
    [SerializeField] private Toggle _topVideoToggle;

    private Navigation _audioNavigation = new Navigation();
    private Navigation _videoNavigation = new Navigation();
    private Navigation _gameplayNavigation = new Navigation();
    

    private ColorBlock _notFocusedColors;

    private ColorBlock _yesFocusedColors;

    /// <summary>
    /// setup references
    /// </summary>
    private void Start()
    {
        _yesFocusedColors = _audioButton.colors;
        _notFocusedColors = _videoButton.colors;

        _audioNavigation = _audioButton.navigation;
        _audioNavigation.mode = Navigation.Mode.Explicit;
        _videoNavigation = _videoButton.navigation;
        _videoNavigation.mode = Navigation.Mode.Explicit;
        _gameplayNavigation = _gameplayButton.navigation;
        _gameplayNavigation.mode = Navigation.Mode.Explicit; 

    }

    /// <summary>
    /// changes active state of menu elements and button colors when a button is pressed
    /// </summary>
    /// <param name="pageToEnable"></param>
    public void SwitchSettingsPage(int pageToEnable)
    {
        //switch one on and the others off
        foreach(GameObject page in _settingsPages)
        {
            page.SetActive(false);
        }
        _settingsPages[pageToEnable].SetActive(true);

        //for changing buttons colors
        switch (pageToEnable)
        {
            case 0:
                //audio settings button pressed
                _audioButton.colors = _yesFocusedColors;
                _audioNavigation.selectOnDown = _topAudioSlider;
                _audioButton.navigation = _audioNavigation;
                
                _videoButton.colors = _notFocusedColors;
                _videoNavigation.selectOnDown = _topAudioSlider;
                _videoButton.navigation = _videoNavigation;
                
                _gameplayButton.colors = _notFocusedColors;
                _gameplayNavigation.selectOnDown = _topAudioSlider;
                _gameplayButton.navigation = _gameplayNavigation;
                break;
            case 1:
                //video settings button pressed
                _audioButton.colors = _notFocusedColors;
                _audioNavigation.selectOnDown = _topVideoToggle;
                _audioNavigation.selectOnDown = _topVideoToggle;
                
                _videoButton.colors = _yesFocusedColors;
                _videoNavigation.selectOnDown = _topVideoToggle;
                _videoButton.navigation = _videoNavigation;

                
                _gameplayButton.colors = _notFocusedColors;
                _gameplayNavigation.selectOnDown = _topVideoToggle;
                _gameplayButton.navigation = _gameplayNavigation;
                break;
            case 2:
                //gameplay settings button pressed
                _audioButton.colors = _notFocusedColors;
                _audioNavigation.selectOnDown = _topGameplayToggle;
                _audioNavigation.selectOnDown = _topVideoToggle;
                
                _videoButton.colors = _notFocusedColors;
                _videoNavigation.selectOnDown = _topGameplayToggle;
                _videoButton.navigation = _videoNavigation;
                
                _gameplayButton.colors = _yesFocusedColors;
                _gameplayNavigation.selectOnDown = _topGameplayToggle;
                _gameplayButton.navigation = _gameplayNavigation;
                break;
            default:
                //this should not happen
                break;
        }
    }
}
