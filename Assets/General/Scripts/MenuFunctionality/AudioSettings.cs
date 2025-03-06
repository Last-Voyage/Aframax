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

    private bool _hasSceneLoaded = false;

    /// <summary>
    /// When the script is enabled, set the relative sliders to the correct values
    /// </summary>
    private void OnEnable()
    {
        // For each of the individual sliders, if they exist, set the values
        if (_masterSlider != null)
        {
            _masterSlider.value = SaveManager.Instance.GetGameSaveData().CurrentMasterVolume;
        }
        if (_sfxSlider != null)
        {
            _sfxSlider.value = SaveManager.Instance.GetGameSaveData().CurrentSfxVolume;
        }
        if (_ambienceSlider != null)
        {
            _ambienceSlider.value = SaveManager.Instance.GetGameSaveData().CurrentAmbienceVolume;
        }
        if (_voiceSlider != null)
        {
            _voiceSlider.value = SaveManager.Instance.GetGameSaveData().CurrentVoiceVolume;
        }
        if (_musicSlider != null)
        {
            _musicSlider.value = SaveManager.Instance.GetGameSaveData().CurrentMusicVolume;
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
        float masterVolume = _masterSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(masterVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(
                FmodSfxEvents.Instance.MasterVolumeSettingsChanged, Vector3.zero);
        }

        //Saves the audio changes
        SaveManager.Instance.GetGameSaveData().CurrentMasterVolume = masterVolume;
        SaveManager.Instance.SaveText();
    }

    /// <summary>
    /// Change the sfx volume to the corresponding slider
    /// </summary>
    public void ChangeSfx()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/SFXVCA");
        float sfxVolume = _sfxSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(sfxVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.SfxVolumeSettingsChanged, Vector3.zero);
        }

        //Saves the audio changes
        SaveManager.Instance.GetGameSaveData().CurrentSfxVolume = sfxVolume;
        SaveManager.Instance.SaveText();
    }

    /// <summary>
    /// Change the ambience volume to the corresponding slider
    /// </summary>
    public void ChangeAmbience()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/AmbianceVCA");
        float ambienceVolume = _ambienceSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(ambienceVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(
                FmodSfxEvents.Instance.AmbienceVolumeSettingsChanged, Vector3.zero);
        }

        //Saves the audio changes
        SaveManager.Instance.GetGameSaveData().CurrentAmbienceVolume = ambienceVolume;
        SaveManager.Instance.SaveText();
    }

    /// <summary>
    /// Change the voice volume to the corresponding slider
    /// </summary>
    public void ChangeVoice()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/DialogueVCA");
        float voiceVolume = _voiceSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(voiceVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(
                FmodSfxEvents.Instance.VoiceVolumeSettingsChanged, Vector3.zero);
        }

        //Saves the audio changes
        SaveManager.Instance.GetGameSaveData().CurrentVoiceVolume = voiceVolume;
        SaveManager.Instance.SaveText();
    }

    /// <summary>
    /// Change the music volume to the corresponding slider
    /// </summary>
    public void ChangeMusic()
    {
        // Set the VCA to the correct volume to change
        _activeVca = FMODUnity.RuntimeManager.GetVCA("vca:/MusicVCA");
        float musicVolume = _musicSlider.value;
        // Set the volume and save the data
        _activeVca.setVolume(musicVolume);

        if (_hasSceneLoaded)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(
                FmodSfxEvents.Instance.MusicVolumeSettingsChanged, Vector3.zero);
        }

        //Saves the audio changes
        SaveManager.Instance.GetGameSaveData().CurrentMusicVolume = musicVolume;
        SaveManager.Instance.SaveText();
    }
}
