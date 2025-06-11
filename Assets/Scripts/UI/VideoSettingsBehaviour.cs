/**********************************************************************************************************************
// File Name :         VideoSettingsBehaviour.cs
// Author :            Jeremiah Peters
// Contributors :      Andrew Stapay, Nick Rice
// Creation Date :     2/28/2025
// 
// Brief Description : Handles the video settings and applying them
**********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// operates video settings, currently just brightness but probably more to come
/// </summary>
public class VideoSettingsBehaviour : MonoBehaviour
{
    private VolumeProfile _volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustmentsName;

    [Tooltip("Should not be higher than like 5 or some visuals kinda break")]
    [SerializeField] private float _brightnessMultiplier = 3;

    [SerializeField] private Slider _brightnessSlider;

    [SerializeField] private Toggle _subtitleToggleButton;

    [SerializeField] private Toggle _goreToggleButton;

    [SerializeField] private Toggle _fullScreenButton;
    
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    
    private int _resolutionWidth, _resolutionHeight;
    
    /// <summary>
    /// set up references
    /// </summary>
    private void Awake()
    {
        _volumeProfile = GameObject.Find("GlobalVolumePostProcessing").GetComponent<Volume>()?.sharedProfile;

        //even though this line is just an error check, everything breaks without it.
        if (!_volumeProfile.TryGet(out _colorAdjustmentsName))
        {
            throw new System.NullReferenceException(nameof(_colorAdjustmentsName));
        }
        
        AddResolutionsToDropdown();

        _resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
        
        //remembers previously set values 
        _brightnessSlider.value = SaveManager.Instance.GetGameSaveData().GetBrightness();
        _colorAdjustmentsName.postExposure.Override(_brightnessSlider.value * _brightnessMultiplier);

        _subtitleToggleButton.isOn = SaveManager.Instance.GetGameSaveData().IsSubtitlesOn;
        _goreToggleButton.isOn = SaveManager.Instance.GetGameSaveData().IsGoreOn;
    }

    /// <summary>
    /// change brightness value to match slider
    /// </summary>
    public void ChangeBrightness()
    {
        _colorAdjustmentsName.postExposure.Override(_brightnessSlider.value * _brightnessMultiplier);
        
        SaveManager.Instance.GetGameSaveData().SetBrightness(_brightnessSlider.value);
    }

    /// <summary>
    /// updates the setting when the button is pressed
    /// </summary>
    public void ToggleSubtitleSetting()
    {
        SaveManager.Instance.GetGameSaveData().IsSubtitlesOn = _subtitleToggleButton.isOn;
        //stop any current subtitles
        if (!FindObjectOfType<DialoguePopUps>().IsUnityNull())
        {
            FindObjectOfType<DialoguePopUps>().UpdateSubtitleSettingState();
        }
    }

    /// <summary>
    /// updates the setting when the button is pressed
    /// </summary>
    public void ToggleGoreSetting()
    {
        SaveManager.Instance.GetGameSaveData().IsGoreOn = _goreToggleButton.isOn;
    }

    /// <summary>
    /// This inverts whether the game is full screened
    /// </summary>
    public void ToggleFullScreenSetting()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    
    #region Resolution Functions

    /// <summary>
    /// This changes the player's resolution
    /// </summary>
    /// <param name="newResolutionPointer">The pointer that picks the selected resolution option</param>
    private void ChangeResolution(int newResolutionPointer)
    {
        StoreWidthHeight(_resolutionDropdown.options[newResolutionPointer].text);
        
        Screen.SetResolution(_resolutionWidth,_resolutionHeight, Screen.fullScreen);
    }
    
    /// <summary>
    /// This adds all the resolutions as options to the dropdown
    /// </summary>
    private void AddResolutionsToDropdown()
    {
        List<TMP_Dropdown.OptionData> optionData = new List<TMP_Dropdown.OptionData>();

        string tempOption = "";
        string lastAddition = "lastVoyageRocks"; // This shouldn't be an empty string because tempOption is also empty
        
        _resolutionDropdown.ClearOptions();

        optionData.Capacity = Screen.resolutions.Length;
        
        // This goes through each of the available screen resolutions
        foreach (var resolution in Screen.resolutions)
        {
            // Grabs the name of the resolution
            tempOption = resolution.ToString();
            // Cuts out the refresh rate
            tempOption = tempOption[..(tempOption.LastIndexOf('@') - 1)];

            if (lastAddition == tempOption)
            {
                continue;
            }

            lastAddition = tempOption;
            // And then adds it as an option to a list
            TMP_Dropdown.OptionData testerOptionData = new TMP_Dropdown.OptionData(tempOption);
            optionData.Add(testerOptionData);
            
        }
        // Finally filling out the actual options with the list made above
        _resolutionDropdown.options = optionData;
    }

    /// <summary>
    /// This takes the default resolution, and makes it the first selected option
    /// </summary>
    private void SelectDefaultResolution()
    {
        int selectedResolution = 0;
        string currentResolutionString = Screen.currentResolution.ToString();
        
        // This cuts off the refresh rate of the current resolution
        currentResolutionString = currentResolutionString[..(currentResolutionString.LastIndexOf('@')-1)];

        // This goes through the resolutions and makes it the currently selected option
        for (int i = 0; i < _resolutionDropdown.options.Count; i++)
        {
            if (String.Equals(_resolutionDropdown.options[i].text,currentResolutionString))
            {
                selectedResolution = i;
            }
        }

        StoreWidthHeight(currentResolutionString);
        _resolutionDropdown.value = selectedResolution;
        _resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Stores the width and height of the current resolution
    /// </summary>
    /// <param name="resolution">The resolution to be broken into width and height</param>
    private void StoreWidthHeight(string resolution)
    {
        int seperatorIndex = resolution.LastIndexOf('x');
        _resolutionWidth = int.Parse(resolution[..(seperatorIndex-1)]);
        _resolutionHeight = int.Parse(resolution[(seperatorIndex+2)..]);
    }
    
    #endregion

    /// <summary>
    /// Prevents the top resolution from always being the first selected
    /// </summary>
    private void OnEnable()
    {
        SelectDefaultResolution();
    }

    /// <summary>
    /// This removes the listener to check if the resolution has changed
    /// </summary>
    private void OnDisable()
    {
        _resolutionDropdown.onValueChanged.RemoveListener(ChangeResolution);
    }
}
