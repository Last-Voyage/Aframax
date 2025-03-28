/**********************************************************************************************************************
// File Name :         VideoSettingsBehaviour.cs
// Author :            Jeremiah Peters
// Contributors :      Andrew Stapay
// Creation Date :     2/28/2025
// 
// Brief Description : Handles the video settings and applying them
**********************************************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

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

        ///remember settings
        _brightnessSlider.value = SaveManager.Instance.GetGameSaveData().GetBrightness();
        _colorAdjustmentsName.postExposure.Override(_brightnessSlider.value * _brightnessMultiplier);

        Debug.Log(SaveManager.Instance.GetGameSaveData().IsSubtitlesOn);
        _subtitleToggleButton.isOn = SaveManager.Instance.GetGameSaveData().IsSubtitlesOn;
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
    /// 
    /// </summary>
    /// <param name="subtitleState">bool</param>
    public void ToggleSubtitleSetting()
    {
        SaveManager.Instance.GetGameSaveData().IsSubtitlesOn = _subtitleToggleButton.isOn;
    }
}
