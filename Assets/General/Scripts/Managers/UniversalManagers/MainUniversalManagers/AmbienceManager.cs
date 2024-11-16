/******************************************************************************
// File Name:       AmbienceManager.cs
// Author:          Andrea Swihart-DeCoster
// Contributors:    Ryan Swanson
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
    public static AmbienceManager Instance; 
    public FModEventReference IntervalEvent;
    private List<EventInstance> _allAmbientEvents;

    /// <summary>
    /// Performs any set up needed for the manager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        
        StartGameBackgroundAudio();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        AframaxSceneManager.Instance.GetOnSceneChanged.AddListener(StartGameBackgroundAudio);
    }

    /// <summary>
    /// Establishes the instance for the ambience manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;

        //Established the instance for the FmodAmbienceEvents
        GetComponent<FmodAmbienceEvents>().SetUpInstance();
    }

    public void PlayintervalAudio()
    {
        StartCoroutine(IntervalEvent.RandomAmbienceLoop());
    }

    /// <summary>
    /// Starts playing continuous background audio in game scenes
    /// </summary>
    private void StartGameBackgroundAudio()
    {
        _allAmbientEvents = new List<EventInstance>();

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
