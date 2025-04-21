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

    [SerializeField] private string _GameplaySettingFilePath;

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
        SensitivitySlider.onValueChanged.AddListener(delegate { SaveData(); });
        _invertX.onValueChanged.AddListener(delegate { SaveData(); });
        _invertY.onValueChanged.AddListener(delegate { SaveData(); });

        string[] camSettings = File.ReadAllLines(Application.streamingAssetsPath +
            _GameplaySettingFilePath)[0].Split(" ");

        SensitivitySlider.value = float.Parse(camSettings[0]);
        _invertX.isOn = bool.Parse(camSettings[1]);
        _invertY.isOn = bool.Parse(camSettings[2]);
    }

    /// <summary>
    /// happens when the game object is disabled
    /// save changes and remove listeners
    /// </summary>
    private void OnDisable()
    {
        //save data one last time
        SaveData();
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
    /// Save the values to the save file
    /// </summary>
    private void SaveData()
    {
        // Convert the sensitivity to a string
        string _settings = SensitivitySlider.value + " " + _invertX.isOn + " " + _invertY.isOn;

        // Write the text to the file
        File.WriteAllText(Application.streamingAssetsPath + _GameplaySettingFilePath, _settings);
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
