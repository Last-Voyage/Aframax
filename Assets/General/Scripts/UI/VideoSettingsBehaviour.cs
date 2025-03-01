/**********************************************************************************************************************
// File Name :         VideoSettingsBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     2/28/2025
// 
// Brief Description : Handles the video settings and applying them (presently just brightness)
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class VideoSettingsBehaviour : MonoBehaviour
{
    //public UnityEngine.Rendering.Universal.ColorAdjustments thename;

    private UnityEngine.Rendering.Volume volume;
    private UnityEngine.Rendering.VolumeProfile volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustmentsName;

    [Tooltip("Should not be higher than like 5 at most or visuals break")]
    [SerializeField] private float _maximumBrightness;

    /// <summary>
    /// set up references
    /// </summary>
    private void Awake()
    {
        volume = GameObject.Find("GlobalVolumePostProcessing").GetComponent<Volume>();
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
        //something with save data to keep slider position?
    }
}
