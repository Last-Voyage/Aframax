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
public class PersistentSoundManager : AudioManager
{
    [SerializeField] private EventReference _ambientSound;

    private void Start()
    {
        StartBackgroundAudio();
    }

    /// <summary>
    /// Starts playing an continuous background audio
    /// </summary>
    private void StartBackgroundAudio()
    {
        PlayPersistentAudio(_ambientSound);
    }

    /// <summary>
    /// Starts an instance of the persistent audio to play
    /// </summary>
    /// <param name="eventReference"></param>
    private void PlayPersistentAudio(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.start();

        eventInstance.release();
    }
}
