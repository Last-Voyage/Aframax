/*****************************************************************************
// File Name :         GamepleySettings.cs
// Author :            Nabil Tagba
// Creation Date :     2/27/2025
//
// Brief Description : take care of all game play setting functionalities.
//sensitivity, inverts
*****************************************************************************/
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// take care of all game play setting functionalities.
/// sensitivity, inverts
/// </summary>
public class GameplaySettings : MonoBehaviour
{
    private GameObject _camera;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Toggle _invertX;
    [SerializeField] private Toggle _invertY;

    [SerializeField] private string _GameplaySettingFilePath;


    /// <summary>
    /// happens when the game object is enabled
    /// </summary>
    private void OnEnable()
    {
        _sensitivitySlider.onValueChanged.AddListener(delegate { SaveData(); });
        _invertX.onValueChanged.AddListener(delegate { SaveData(); });
        _invertY.onValueChanged.AddListener(delegate { SaveData(); });

        string[] camSettings = File.ReadAllLines(Application.streamingAssetsPath +
            _GameplaySettingFilePath)[0].Split(" ");

        _sensitivitySlider.value = float.Parse(camSettings[0]);
        _invertX.isOn = bool.Parse(camSettings[1]);
        _invertY.isOn = bool.Parse(camSettings[2]);

        try
        {
            FindObjectOfType<CamSettings>().wasSettingsChanged = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    /// <summary>
    /// happens when the game object is disabled
    /// </summary>
    private void OnDisable()
    {
        _sensitivitySlider.onValueChanged.RemoveAllListeners();
        _invertX.onValueChanged.RemoveAllListeners();
        _invertY.onValueChanged.RemoveAllListeners();
    }

  

    /// <summary>
    /// Save the volumes to the save file
    /// </summary>
    private void SaveData()
    {
        // Convert the sensitivity to a string
        string _settings = _sensitivitySlider.value + " " + _invertX.isOn + " " + _invertY.isOn;

        // Write the text to the file
        File.WriteAllText(Application.streamingAssetsPath + _GameplaySettingFilePath, _settings);
    }







}
