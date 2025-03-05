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
    private VolumeProfile volumeProfile;
    private UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustmentsName;

    [Tooltip("Should not be higher than like 5 or some visuals kinda break")]
    [SerializeField] private float _maximumBrightness;

    [SerializeField] private Slider _brightnessSlider;

    /// <summary>
    /// set up references
    /// </summary>
    private void Awake()
    {
        volumeProfile = GameObject.Find("GlobalVolumePostProcessing").GetComponent<Volume>()?.sharedProfile;

        //even though this line is just an error check, everything breaks without it.
        if (!volumeProfile.TryGet(out colorAdjustmentsName))
        {
            throw new System.NullReferenceException(nameof(colorAdjustmentsName));
        }

        _brightnessSlider.value = SaveManager.Instance.GetGameSaveData().GetBrightness();
    }

    /// <summary>
    /// change brightness value to match slider
    /// </summary>
    public void ChangeBrightness()
    {
        colorAdjustmentsName.postExposure.Override(_brightnessSlider.value * _maximumBrightness);
        
        SaveManager.Instance.GetGameSaveData().SetBrightness(_brightnessSlider.value);
    }
}
