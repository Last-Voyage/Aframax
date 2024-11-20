using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using System.IO;

public class AudioSettings : MonoBehaviour
{
    private VCA _vca;

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

    private void OnEnable()
    {
        float[] volumes = LoadData();

        _masterSlider.value = volumes[0];
        _sfxSlider.value = volumes[1];
        _ambienceSlider.value = volumes[2];
        _voiceSlider.value = volumes[3];
        _musicSlider.value = volumes[4];
    }

    public void ChangeMaster()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/MasterVCA");
        _masterVolume = _masterSlider.value;
        _vca.setVolume(Mathf.Pow(10.0f, _masterVolume / 20f));
        SaveData();
    }

    public void ChangeSfx()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/SFXVCA");
        _sfxVolume = _sfxSlider.value;
        _vca.setVolume(Mathf.Pow(10.0f, _sfxVolume / 20f));
        SaveData();
    }

    public void ChangeAmbience()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/AmbianceVCA");
        _ambienceVolume = _ambienceSlider.value;
        _vca.setVolume(Mathf.Pow(10.0f, _ambienceVolume / 20f));
        SaveData();
    }

    public void ChangeVoice()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/DialogueVCA");
        _voiceVolume = _voiceSlider.value;
        _vca.setVolume(Mathf.Pow(10.0f, _voiceVolume / 20f));
        SaveData();
    }

    public void ChangeMusic()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/MusicVCA");
        _musicVolume = _musicSlider.value;
        _vca.setVolume(Mathf.Pow(10.0f, _musicVolume / 20f));
        SaveData();
    }

    private float[] LoadData()
    {
        float[] volumeArray = new float[5];

        string[] volumeStrings = File.ReadAllLines(Application.streamingAssetsPath + _audioSettingsFilePath)[0].Split(" ");

        for (int i = 0; i < volumeArray.Length; i++)
        {
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

    private void SaveData()
    {
        string volumeSaveFile = _masterVolume + " " + _sfxVolume + " " + _ambienceVolume
            + " " + _voiceVolume + " " + _musicVolume;

        File.WriteAllText(Application.streamingAssetsPath + _audioSettingsFilePath, volumeSaveFile);
    }
}
