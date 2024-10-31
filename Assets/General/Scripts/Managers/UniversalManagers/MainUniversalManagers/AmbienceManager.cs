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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages audio that persists throughout the game
/// </summary>
public class AmbienceManager : AudioManager
{
    public static new AmbienceManager Instance;

    private List<EventInstance> _allAmbientEvents;

    private void Awake()
    {
        // Parent needs to be removed at runtime in order for DontDestroyOnLoad to work

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _allAmbientEvents = new List<EventInstance>();
        StartBackgroundAudio();
        SubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        SceneLoadingManager.Instance.GetSceneChangedEvent.AddListener(StartBackgroundAudio);
    }

    /// <summary>
    /// Starts playing an continuous background audio
    /// </summary>
    private void StartBackgroundAudio()
    {
        //Ambience manager should only play in game scenes, not the main menu.
        if (SceneManager.GetActiveScene().buildIndex == SceneLoadingManager.Instance.MainMenuSceneIndex)
        {
            foreach (var sound in _allAmbientEvents)
            {
                sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            return;
        }
        foreach (var sound in FmodAmbienceEvents.Instance.AmbientBackgroundSounds)
        {
            if(sound.IsNull)
            {
                return;
            }
            StartAmbience(sound);
        }
    }

    /// <summary>
    /// Starts an instance of the persistent audio to play
    /// </summary>
    /// <param name="eventReference">fmod event reference to play</param>
    private void StartAmbience(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        _allAmbientEvents.Add(eventInstance);
        eventInstance.start();
        eventInstance.release();
    }
}
