/**********************************************************************************************************************
// File Name :         VideoSettingsBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     2/28/2025
// 
// Brief Description : Handles the video settings and applying them
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// operates video settings, currently just brightness but probably more to come
/// </summary>
public class VideoSettingsBehaviour : MonoBehaviour
{
    private VolumeProfile _volumeProfile;
    private ColorAdjustments _colorAdjustmentsName;

    [Tooltip("Should not be higher than like 5 or some visuals kinda break")]
    [Range(0.2f, 5)]
    [SerializeField] private float _maximumBrightness;

    /// <summary>
    /// set up references
    /// </summary>
    private void Awake()
    {
        _volumeProfile = GameObject.Find("GlobalVolumePostProcessing").GetComponent<UnityEngine.Rendering.Volume>()?.profile;

        //even though this line is just an error check, everything breaks without it.
        if (!_volumeProfile.TryGet(out _colorAdjustmentsName))
        {
            throw new System.NullReferenceException(nameof(_colorAdjustmentsName));
        }
    }

    /// <summary>
    /// change brightness value to match slider
    /// </summary>
    public void ChangeBrightness(Slider BrightnessSlider)
    {
        _colorAdjustmentsName.postExposure.Override(BrightnessSlider.value * _maximumBrightness);
        //needs something with save data to keep slider position
    }
}
