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

/// <summary>
/// operates video settings, currently just brightness but probably more to come
/// </summary>
public class VideoSettingsBehaviour : MonoBehaviour
{
    private VolumeProfile volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustmentsName;

    [Tooltip("Should not be higher than like 5 or some visuals kinda break")]
    [SerializeField] private float _maximumBrightness;

    /// <summary>
    /// set up references
    /// </summary>
    private void Awake()
    {
        volumeProfile = GameObject.Find("GlobalVolumePostProcessing").GetComponent<UnityEngine.Rendering.Volume>()?.profile;

        //even though this line is just an error check, everything breaks without it.
        if (!volumeProfile.TryGet(out colorAdjustmentsName)) throw new System.NullReferenceException(nameof(colorAdjustmentsName));
    }

    /// <summary>
    /// change brightness value to match slider
    /// </summary>
    public void ChangeBrightness(Slider BrightnessSlider)
    {
        colorAdjustmentsName.postExposure.Override(BrightnessSlider.value * _maximumBrightness);
        //needs something with save data to keep slider position
    }
}
