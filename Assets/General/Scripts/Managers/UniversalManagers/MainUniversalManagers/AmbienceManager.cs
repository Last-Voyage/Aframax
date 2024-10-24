/******************************************************************************
// File Name:       PersistentSoundManager.cs
// Author:          Andrea Swihart-DeCoster
// Creation Date:   October 1st, 2024
//
// Description:     Manages any sound that player persistently throughout the
                    game. e.g music, background audio, etc
******************************************************************************/

using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Manages audio that persists throughout the game
/// </summary>
public class AmbienceManager : AudioManager
{
    public static new AmbienceManager Instance;

    private void Awake()
    {
        // Parent needs to be removed at runtime in order for DontDestroyOnLoad to work
        transform.parent = null;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        
    }

    private void Start()
    {
        StartBackgroundAudio();
    }

    /// <summary>
    /// Starts playing an continuous background audio
    /// </summary>
    private void StartBackgroundAudio()
    {
        foreach(var sound in FmodAmbienceEvents.Instance.AmbientBackgroundSounds)
        {
            StartAmbience(sound);
        }
    }

    /// <summary>
    /// Starts an instance of the persistent audio to play
    /// </summary>
    /// <param name="eventReference"></param>
    private void StartAmbience(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.start();
        eventInstance.release();
    }
}
