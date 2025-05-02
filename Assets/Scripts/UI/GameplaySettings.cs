/*****************************************************************************
// File Name :         GameplaySettings.cs
// Author :            Nabil Tagba
// Contributor :       Ryan Swanson
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
    
    [SerializeField] private Toggle _controllerToggleButton;

    private bool _isPreventingUIChange = false;
    private WaitForEndOfFrame _waitToAllowUIChange = new WaitForEndOfFrame();

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
        
        if (_controllerToggleButton.isOn != UiManager.IsUsingController)
        {
            _isPreventingUIChange = true;

            StartCoroutine(PreventUISwap());

            _controllerToggleButton.isOn = !_controllerToggleButton.isOn;
        }
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
        _invertX.onValueChanged.AddListener(delegate { ToggleCameraXInvert(); });
        _invertY.onValueChanged.AddListener(delegate { ToggleCameraYInvert(); });

        SensitivitySlider.value = SaveManager.Instance.GetGameSaveData().CameraSensitivty;
        _invertX.isOn = SaveManager.Instance.GetGameSaveData().IsCameraXAxisInverted;
        _invertY.isOn = SaveManager.Instance.GetGameSaveData().IsCameraYAxisInverted;
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
        _invertX.onValueChanged.RemoveAllListeners();
        _invertY.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// updates the setting for the camera X invert when the button is pressed
    /// </summary>
    private void ToggleCameraXInvert()
    {
        SaveManager.Instance.GetGameSaveData().IsCameraXAxisInverted = _invertX.isOn;
    }

    /// <summary>
    /// updates the setting for the camera Y invert when the button is pressed
    /// </summary>
    private void ToggleCameraYInvert()
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

    /// <summary>
    /// Updates the UI in the game to reflect controller or keyboard inputs
    /// </summary>
    public void ToggleControllerSetting()
    {
        if (!_isPreventingUIChange)
        {
            UiManager.Instance.SwapInput();
        }
    }

    /// <summary>
    /// This is meant to prevent the UI from swapping because Unity's system means that this will be called
    /// on a value change
    /// </summary>
    /// <returns>A singular frame</returns>
    private IEnumerator PreventUISwap()
    {
        yield return _waitToAllowUIChange;
        _isPreventingUIChange = false;
    }
}
