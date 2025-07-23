/*****************************************************************************
// File Name :         GameplaySettings.cs
// Author :            Nabil Tagba
// Contributor :       Ryan Swanson, Jeremiah Peters, Nick Rice
// Creation Date :     2/27/2025
//
// Brief Description : take care of all game play setting functionalities.
//sensitivity, inverts
*****************************************************************************/
using System;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// take care of all game play setting functionalities.
/// sensitivity, inverts
/// </summary>
public class GameplaySettings : MonoBehaviour
{
    public Slider SensitivitySlider;
    [SerializeField] private Toggle _invertX;
    [SerializeField] private Toggle _invertY;

    private bool _isPreventingUIChange = false;

    public static GameplaySettings Instance;

    /// <summary>
    /// happens on awake, used to set the instance
    /// </summary>
    private void Awake()
    {
        if (Instance.IsUnityNull())
        {
            Instance = this;
        }

        //remembers previously set values 
        _invertX.isOn = SaveManager.Instance.GetGameSaveData().IsCameraXAxisInverted;
        _invertY.isOn = SaveManager.Instance.GetGameSaveData().IsCameraYAxisInverted;
    }
    /// <summary>
    /// happens when the game object is enabled
    /// save changes when values are changed
    /// </summary>
    private void OnEnable()
    {
        //set max sensitivity
        SensitivitySlider.maxValue = SaveManager.Instance.MaxSensitivity;

        //save data when the values are changed
        SensitivitySlider.onValueChanged.AddListener(delegate { SetCameraSensitivity(); });

        SensitivitySlider.value = SaveManager.Instance.GetGameSaveData().CameraSensitivty;
    }

    /// <summary>
    /// happens when the game object is disabled
    /// save changes and remove listeners
    /// </summary>
    private void OnDisable()
    {
        //save data one last time
        try
        {
            CameraSettings.WasSettingsChanged?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
        //remove listeners
        SensitivitySlider.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// updates the setting for the camera X invert when the button is pressed
    /// </summary>
    public void ToggleCameraXInvert()
    {
        SaveManager.Instance.GetGameSaveData().IsCameraXAxisInverted = _invertX.isOn;
    }

    /// <summary>
    /// updates the setting for the camera Y invert when the button is pressed
    /// </summary>
    public void ToggleCameraYInvert()
    {
        SaveManager.Instance.GetGameSaveData().IsCameraYAxisInverted = _invertY.isOn;
    }

    /// <summary>
    /// updates the settings for the camera sensitivity
    /// </summary>
    private void SetCameraSensitivity()
    {
        SaveManager.Instance.GetGameSaveData().CameraSensitivty = SensitivitySlider.value;
    }
}
