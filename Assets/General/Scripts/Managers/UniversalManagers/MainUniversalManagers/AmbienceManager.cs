/******************************************************************************
// File Name:       PersistentSoundManager.cs
// Author:          Andrea Swihart-DeCoster
// Creation Date:   October 1st, 2024
//
// Description:     Manages any sound that player persistently throughout the
                    game. e.g. music, background audio, etc
******************************************************************************/

using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;

/// <summary>
/// Manages audio that persists throughout the game
/// </summary>
public class AmbienceManager : AudioManager
{
    public static new AmbienceManager Instance;

    private List<EventInstance> _allAmbientEvents;

    private void Awake()
    {
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
        StartGameBackgroundAudio();
        SubscribeToEvents();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        AframaxSceneManager.Instance.GetOnSceneChanged.AddListener(StartGameBackgroundAudio);
    }

    /// <summary>
    /// Starts playing continuous background audio in game scenes
    /// </summary>
    private void StartGameBackgroundAudio()
    {
        // Stop any instances of music playing
        foreach (var sound in _allAmbientEvents)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        // Ambience Manager should not play outside of game scenes
        if (!AframaxSceneManager.Instance.IsGameScene())
        {
            return;
        }
        
        // Loop through and play all ambient game sounds
        foreach (var sound in FmodAmbienceEvents.Instance.AmbientGameBackgroundSounds)
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
