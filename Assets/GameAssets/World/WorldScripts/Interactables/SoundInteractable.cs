/******************************************************************************
// File Name:       SoundInteractable.cs
// Author:          Nick Rice
// Creation Date:   February 4th, 2025
//
// Description:     This radio plays 
******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundInteractable : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private EventReference _actualSound;

    [SerializeField]
    private float _secondsBetweenPlayingSound;
    private WaitForSeconds _sfxTotalTime;
    
    [SerializeField] 
    private float _secondsBeforeRestarting;
    private WaitForSeconds _waitForRestarting;
    
    private IEnumerator _makeNoise;
    private IEnumerator _playerCausedSilence;

    /// <summary>
    /// Initializes the wait for seconds with design specified wait times
    /// Starts playing sound
    /// </summary>
    private void Start()
    {
        _sfxTotalTime = new WaitForSeconds(_secondsBetweenPlayingSound);
        _waitForRestarting = new WaitForSeconds(_secondsBeforeRestarting);

        _makeNoise = PlayLocalNoise();
        StartCoroutine(_makeNoise);
    }
    
    /// <summary>
    /// if (Silence) {(Stop Silence)}
    /// 
    /// if (!Playing Sound) {(Play Sound)}
    /// else {(Stop Playing Sound)}
    /// </summary>
    public void OnSoundChange()
    {
        if (!_playerCausedSilence.IsUnityNull())
        {
            StopCoroutine(_playerCausedSilence);
            _playerCausedSilence = null;
        }
        if (_makeNoise.IsUnityNull())
        {
            _makeNoise = PlayLocalNoise();
            StartCoroutine(_makeNoise);
        }
        else
        {
            StopCoroutine(_makeNoise);
            _makeNoise = null;
            _playerCausedSilence = Silence();
            StartCoroutine(_playerCausedSilence);
        }
    }

    /// <summary>
    /// Plays sound every x seconds
    /// </summary>
    private IEnumerator PlayLocalNoise()
    {
        while (true)
        {
            RuntimeSfxManager.APlayOneShotSfx?.Invoke(_actualSound,gameObject.transform.position);
            yield return _sfxTotalTime;
        }
    }

    /// <summary>
    /// Waits to restart the sound after x seconds
    /// </summary>
    private IEnumerator Silence()
    {
        yield return _waitForRestarting;
        OnSoundChange();
    }
}
