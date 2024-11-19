using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FMOD.Studio;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _ambienceSlider;
    [SerializeField] private Slider _voiceSlider;
    [SerializeField] private AudioMixer _mixer;

    private float _masterVolume;
    private float _sfxVolume;
    private float _ambienceVolume;
    private float _voiceVolume;

    void FixedUpdate()
    {
        if (_masterSlider != null)
        {
            _masterVolume = _masterSlider.value;
        }
        if (_sfxSlider != null)
        {
            _sfxVolume = _sfxSlider.value;
        }
        if (_ambienceSlider != null)
        {
            _ambienceVolume = _ambienceSlider.value;
        }
        if (_voiceSlider != null)
        {
            _voiceVolume = _voiceSlider.value;
        }

        _mixer.SetFloat("Master", Mathf.Log10(_masterVolume) * 20);
        _mixer.SetFloat("Ambience", Mathf.Log10(_ambienceVolume) * 20);
        _mixer.SetFloat("SFX", Mathf.Log10(_sfxVolume) * 20);
    }
}
