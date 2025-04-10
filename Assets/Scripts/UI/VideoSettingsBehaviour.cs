/**********************************************************************************************************************
// File Name :         VideoSettingsBehaviour.cs
// Author :            Jeremiah Peters
// Contributors :      Andrew Stapay
// Creation Date :     2/28/2025
// 
// Brief Description : Handles the video settings and applying them
**********************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

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

    [SerializeField] private Dropdown _resolutionDropdown;
    
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
        SelectDefaultResolution();
        
        ///remembers previously set values 
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
        if (FindObjectOfType<DialoguePopUps>() != null)
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

    private void AddResolutionsToDropdown()
    {
        List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();

        string tempOption;
        
        foreach (var resolution in Screen.resolutions)
        {
            tempOption = resolution.ToString();
            tempOption = tempOption[..(tempOption.LastIndexOf('@') - 1)];
            optionData.Add(new Dropdown.OptionData(tempOption));
        }
        
        _resolutionDropdown.options = optionData;
    }

    private void SelectDefaultResolution()
    {
        int selectedResolution = 0;
        string currentResolutionString = Screen.currentResolution.ToString();

        currentResolutionString = currentResolutionString[..(currentResolutionString.LastIndexOf('@')-1)];

        for (int i = 0; i < _resolutionDropdown.options.Capacity; i++)
        {
            if (_resolutionDropdown.options[i].text == currentResolutionString)
            {
                selectedResolution = i;
                return;
            }
        }

        _resolutionDropdown.value = selectedResolution;
    }
}
