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
using System;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

/// <summary>
/// Manages audio that persists throughout the game
/// </summary>
public class AmbienceManager : AudioManager
{
    public static AmbienceManager Instance; 
    
    public static Action<EventReference, GameObject> APlayAmbienceOnObject;
    
    private List<EventInstance> _allAmbientEvents;

    /// <summary>
    /// Performs any set up needed for the manager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        
        SubscribeToActions(true);
        StartGameBackgroundAudio();
    }

    /// <summary>
    /// Subscribes to general events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        
        AframaxSceneManager.Instance.GetOnSceneChanged.AddListener(StartGameBackgroundAudio);
    }

    /// <summary>
    /// Subscribes to gameplay specific events
    /// </summary>
    protected override void SubscribeToGameplayEvents()
    {
        GameStateManager.Instance.GetOnGamePaused().AddListener(StopAllAmbience);
        GameStateManager.Instance.GetOnGameUnpaused().AddListener(StartGameBackgroundAudio);
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
    
    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val"> if true, subscribes </param>
    private void SubscribeToActions(bool val)
    {
        if (val)
        {
            APlayAmbienceOnObject += StartAmbienceOnObject;
            
            return;
        }

        APlayAmbienceOnObject -= StartAmbienceOnObject;
    }

    private void Awake()
    {
        PlayIntervalAudio();
    }
    
    /// <summary>
    /// Starts playing continuous background audio in game scenes
    /// </summary>
    private void StartGameBackgroundAudio()
    {
        if (_allAmbientEvents == null)
        {
            _allAmbientEvents = new List<EventInstance>();
        }
        else
        {
            ClearAmbientAudio();
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
        
        PlayIntervalAudio();
    }
    
    /// <summary>
    /// Stops all ambient audio and clears the list
    /// </summary>
    private void ClearAmbientAudio()
    {
        if(_allAmbientEvents.Capacity > 0)
        {
            // Stop any instances of music playing
            foreach (var sound in _allAmbientEvents)
            {
                sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            
            _allAmbientEvents.Clear();
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        SubscribeToActions(false);
    }

    #region Interval Ambience
    
    /// <summary>
    /// Plays audio at certain intervals
    /// </summary>
    private void PlayIntervalAudio()
    {
        // Loop through and play all ambient game sounds
        foreach (var intervalEvent in FmodAmbienceEvents.Instance.IntervalAmbientEvents)
        {
            intervalEvent.IntervalCoroutine = StartCoroutine(intervalEvent.RandomAmbienceLoop());
        }
    }
    
    /// <summary>
    /// Stops all interval audio sounds
    /// </summary>
    private void StopIntervalAudio()
    {
        // Loop through and play all ambient game sounds
        foreach (var intervalEvent in FmodAmbienceEvents.Instance.IntervalAmbientEvents)
        {
            StopCoroutine(intervalEvent.IntervalCoroutine);
        }
    }
    
    #endregion
    
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

    /// <summary>
    /// Plays an audio effect attached to a game object
    /// </summary>
    /// <param name="eventReference"> event ref for audio to be played </param>
    /// <param name="audioObject"> object the audio effect will be attached to </param>
    /// <returns></returns>
    private void StartAmbienceOnObject(EventReference eventReference, GameObject audioObject)
    {
        if (eventReference.IsNull)
        {
            return;
        }
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        
        RuntimeManager.AttachInstanceToGameObject(eventInstance, audioObject.transform);
        eventInstance.start();
        _allAmbientEvents.Add(eventInstance);
        eventInstance.release();
    }

    /// <summary>
    /// Stops all ambient audio
    /// </summary>
    private void StopAllAmbience()
    {
        // Stop any instances of music playing
        foreach (var sound in _allAmbientEvents)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        
        StopIntervalAudio();
    }
}
