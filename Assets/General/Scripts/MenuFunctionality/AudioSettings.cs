/**********************************************************************************************************************
// File Name :         AudioSettings.cs
// Author :            Charlie Polonus
// Creation Date :     11/17/24
// 
// Brief Description : Handles the audio settings menu, applying it to the VSAs
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using System.IO;

/// <summary>
/// Allows the editing and applying of audio sliders in the settings window to the in-game audio
/// </summary>
public class AudioSettings : MonoBehaviour
{
    private VCA _activeVca;

    [Tooltip("The string representation of the file path to the audio settings save file")]
    [SerializeField] private string _audioSettingsFilePath;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _ambienceSlider;
    [SerializeField] private Slider _voiceSlider;
    [SerializeField] private Slider _musicSlider;

    private float _masterVolume;
    private float _sfxVolume;
    private float _ambienceVolume;
    private float _voiceVolume;
    private float _musicVolume;

    private bool _hasSceneLoaded = false;

    /// <summary>
    /// When the script is enabled, set the relative sliders to the correct values
    /// </summary>
    private void OnEnable()
    {
        // Get all the volumes from the save file
        float[] volumes = LoadData();

        // For each of the individual sliders, if they exist, set the values
        if (_masterSlider != null)
        {
            _masterSlider.value = volumes[0];
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.value = volumes[1];
        }
        if (_ambienceSlider != null)
        {
            _ambienceSlider.value = volumes[2];
        }
        if (_voiceSlider != null)
        {
            _voiceSlider.value = volumes[3];
        }
        if (_musicSlider != null)
        {
            _musicSlider.value = volumes[4];
        }

        //so the sfx don't play before the scene is done loading
        _hasSceneLoaded = true;
    }

    /// <summary>
    /// Change the master volume to the corresponding slider
    /// </summary>
    public void ChangeMaster()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/MasterVCA");
        _masterVolume = _masterSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(_masterVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.MasterVolumeSettingsChanged, Vector3.zero);
        }

        SaveData();
    }

    /// <summary>
    /// Change the sfx volume to the corresponding slider
    /// </summary>
    public void ChangeSfx()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/SFXVCA");
        _sfxVolume = _sfxSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(_sfxVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.SfxVolumeSettingsChanged, Vector3.zero);
        }

        SaveData();
    }

    /// <summary>
    /// Change the ambience volume to the corresponding slider
    /// </summary>
    public void ChangeAmbience()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/AmbianceVCA");
        _ambienceVolume = _ambienceSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(_ambienceVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.AmbienceVolumeSettingsChanged, Vector3.zero);
        }

        SaveData();
    }

    /// <summary>
    /// Change the voice volume to the corresponding slider
    /// </summary>
    public void ChangeVoice()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/DialogueVCA");
        _voiceVolume = _voiceSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(_voiceVolume);
        SaveData();
    }

    /// <summary>
    /// Change the music volume to the corresponding slider
    /// </summary>
    public void ChangeMusic()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/MusicVCA");
        _musicVolume = _musicSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(_musicVolume);
        SaveData();
    }

    /// <summary>
    /// Load the save data from the file, converting it to an array
    /// </summary>
    /// <returns>The array of all volumes</returns>
    private float[] LoadData()
    {
        // Create an array and populate it with the volumes as strings
        float[] volumeArray = new float[5];
        string[] volumeStrings = File.ReadAllLines(Application.streamingAssetsPath + _audioSettingsFilePath)[0].Split(" ");

        // Iterate through each string and convert it to a float
        for (int i = 0; i < volumeArray.Length; i++)
        {
            // If the string is possible to convert to a float, do so
            try
            {
                volumeArray[i] = float.Parse(volumeStrings[i]);
            }
            catch
            {
                volumeArray[i] = 1;
            }
        }

        return volumeArray;
    }

    /// <summary>
    /// Save the volumes to the save file
    /// </summary>
    private void SaveData()
    {
        // Convert the volumes to a string
        string volumeSaveFile = _masterVolume + " " + _sfxVolume + " " + _ambienceVolume
            + " " + _voiceVolume + " " + _musicVolume;

        // Write the text to the file
        File.WriteAllText(Application.streamingAssetsPath + _audioSettingsFilePath, volumeSaveFile);
    }
}
