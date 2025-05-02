/******************************************************************************
// File Name:       PersistentAudioManager.cs
// Author:          Andrea Swihart-DeCoster
// Contributors:    Ryan Swanson, Charlie Polonus
// Creation Date:   October 1st, 2024
//
// Description:     Manages any sound that player persistently throughout the
                    game. e.g. music, background audio, etc
******************************************************************************/

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages audio that persists throughout the game
/// </summary>
public class PersistentAudioManager : AudioManager
{
    public static PersistentAudioManager Instance; 
    
    public static Action<EventReference, GameObject> APlayPersistentAudioOnObject;
    
    private List<EventInstance> _allAmbientEvents;
    private List<Coroutine> _allAmbientCoroutines = new();

    private WaitForSeconds _ambienceFadeInTime;
    private WaitForSeconds _ambienceFadeOutTime;

    private EventReference _currentMusicReference;
    private EventInstance _currentMusicInstance;

    private WaitForSeconds _musicFadeInTime;
    private WaitForSeconds _musicFadeOutTime;

    private static UnityEvent _onBossMusicStarted = new();
    private static UnityEvent _onBossMusicEnded = new();

    private Coroutine _musicFadeInCoroutine;
    private Coroutine _musicFadeOutCoroutine;

    private Coroutine _musicStartCoroutine;

    /// <summary>
    /// Performs any set up needed for the manager
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
        
        SubscribeToActions(true);
        SetUpStartingValues();
        StartGameBackgroundAudio();
    }

    /// <summary>
    /// Subscribes to general events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.AddListener(StartGameBackgroundAudio);
        AframaxSceneManager.Instance.GetOnLeavingGameplayScene.AddListener(StopAllAmbience);
    }

    /// <summary>
    /// Unsubscribes from general events
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        AframaxSceneManager.Instance.GetOnGameplaySceneLoaded.RemoveListener(StartGameBackgroundAudio);
        AframaxSceneManager.Instance.GetOnLeavingGameplayScene.RemoveListener(StopAllAmbience);
    }
                                     
    /// <summary>
    /// Establishes the instance for the ambience manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
        
        //Established the instance for the FmodAmbienceEvents
        GetComponent<FmodPersistentAudioEvents>().SetUpInstance();
    }

    /// <summary>
    /// Sets any values to what they should be on start
    /// </summary>
    private void SetUpStartingValues()
    {
        _ambienceFadeInTime = new WaitForSeconds(FmodPersistentAudioEvents.Instance.AmbienceFadeInTime);
        _ambienceFadeOutTime = new WaitForSeconds(FmodPersistentAudioEvents.Instance.AmbienceFadeOutTime);
        _musicFadeInTime = new WaitForSeconds(FmodPersistentAudioEvents.Instance.MusicFadeInTime);
        _musicFadeOutTime = new WaitForSeconds(FmodPersistentAudioEvents.Instance.MusicFadeOutTime);
    }
    
    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val"> if true, subscribes </param>
    private void SubscribeToActions(bool val)
    {
        if (val)
        {
            APlayPersistentAudioOnObject += StartAmbienceOnObject;
            
            return;
        }

        APlayPersistentAudioOnObject -= StartAmbienceOnObject;
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
        foreach (var sound in FmodPersistentAudioEvents.Instance.PersistentGameBackgroundSounds)
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
        foreach (var intervalEvent in FmodPersistentAudioEvents.Instance.IntervalPersistentEvents)
        {
            intervalEvent.IntervalCoroutine = StartCoroutine(intervalEvent.RandomPersistentAudioLoop());
        }
    }
    
    /// <summary>
    /// Stops all interval audio sounds
    /// </summary>
    private void StopIntervalAudio()
    {
        // Loop through and play all ambient game sounds
        foreach (var intervalEvent in FmodPersistentAudioEvents.Instance.IntervalPersistentEvents)
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

    /// <summary>
    /// Fades in the volume of all current ambience playing
    /// </summary>
    private void FadeInAllAmbience()
    {
        StopAllAmbienceFade();
        for(int i =0 ; i < _allAmbientEvents.Count; i++) 
        {
            _allAmbientCoroutines.Add(StartCoroutine
                (ChangePersistentAudioVolume(_allAmbientEvents[i], 1, false)));
        }
    }

    /// <summary>
    /// Fades out the volume of all current ambience playing
    /// </summary>
    private void FadeOutAllAmbience()
    {
        StopAllAmbienceFade();
        for (int i = 0; i < _allAmbientEvents.Count; i++)
        {
            _allAmbientCoroutines.Add(StartCoroutine
                (ChangePersistentAudioVolume(_allAmbientEvents[i], 0, false)));
        }
    }

    /// <summary>
    /// Stops the process of any ambience currently fading in or out
    /// </summary>
    private void StopAllAmbienceFade()
    {
        if (_allAmbientCoroutines.Count == 0 ||
            _allAmbientCoroutines[0].IsUnityNull())
        {
            return;
        }

        for (int i = 0; i < _allAmbientEvents.Count; i++)
        {
            StopCoroutine(_allAmbientCoroutines[i]);
        }
        _allAmbientCoroutines.Clear();
    }

    /// <summary>
    /// Changes an ambience volume from the current value to an end value
    /// </summary>
    /// <param name="instance"> The ambience we are changing </param>
    /// <param name="endAmbienceVolume"> The end volume we want </param>
    /// <param name="isMusic"> Whether or not we are changing  </param>
    /// <returns>Time</returns>
    private IEnumerator ChangePersistentAudioVolume(EventInstance instance, float endAmbienceVolume,
        bool isMusic)
    {
        float startAmbienceVolume;
        float timeElapsed = 0;
        float fadeTime;

        // Sets the start volume as the current volume
        instance.getVolume(out startAmbienceVolume);

        if(isMusic)
        {
            // Determines the fade time based on if we are fading in or fading out audio for music
            fadeTime = startAmbienceVolume < endAmbienceVolume ?
                FmodPersistentAudioEvents.Instance.MusicFadeInTime : FmodPersistentAudioEvents.Instance.MusicFadeOutTime;
        }
        else
        {
            // Determines the fade time for ambience
            fadeTime = startAmbienceVolume < endAmbienceVolume ?
                FmodPersistentAudioEvents.Instance.AmbienceFadeInTime : FmodPersistentAudioEvents.Instance.AmbienceFadeOutTime;
        }
        

        // The process of changing the volume
        while (timeElapsed < 1)
        {
            // Increments the time elapsed based on the fade time
            timeElapsed += Time.deltaTime / fadeTime;
            // Sets the volume of the ambience based on the current time elapsed
            instance.setVolume(Mathf.Lerp(startAmbienceVolume, endAmbienceVolume, timeElapsed));
            yield return null;
        }
    }

    #region Music
    /// <summary>
    /// Starts playing music via an id as input
    /// </summary>
    /// <param name="id"> The id of the music to start</param>
    public void StartMusicByID(int id)
    {
        // Prevents out of bounds ids
        if(FmodPersistentAudioEvents.Instance.MusicInGame.Length <= id || id < 0)
        {
            return;
        }

        // Starts the music by using the reference found for that id
        StartMusicByReference(FmodPersistentAudioEvents.Instance.MusicInGame[id]);
    }

    /// <summary>
    /// Starts playing music via using a reference as input
    /// </summary>
    /// <param name="reference"> The music to start </param>
    public void StartMusicByReference(EventReference reference)
    {
        // Returns if we are trying to play the music we are already playing
        if (!_currentMusicReference.IsNull && reference.Guid == _currentMusicReference.Guid)
        {
            return;
        }

        // If there is music already trying to fade in, stop doing that
        if (_musicFadeInCoroutine != null)
        {
            StopCoroutine(_musicFadeInCoroutine);
        }
        // If there is music already trying to fade out, stop doing that
        if (_musicFadeOutCoroutine != null)
        {
            StopCoroutine(_musicFadeOutCoroutine);
        }
        // If there is music already trying to start, stop doing that
        if(_musicStartCoroutine != null)
        {
            StopCoroutine(_musicStartCoroutine); 
        }

        // Start the actual desired music
        _musicStartCoroutine = StartCoroutine(StartMusicProcess(reference));
    }

    /// <summary>
    /// Stops any current music and starts up a new music
    /// </summary>
    /// <param name="reference"> The new music to play </param>
    /// <returns> Time </returns>
    private IEnumerator StartMusicProcess(EventReference reference)
    {
        // Checks if there is any music currently playing
        if (_currentMusicInstance.isValid())
        {
            // Starts the process of fading the volume to 0
            _musicFadeOutCoroutine = 
                StartCoroutine(ChangePersistentAudioVolume(_currentMusicInstance, 0,true));
            // Waits for the fade out time
            yield return _musicFadeOutTime;
            _musicFadeOutCoroutine = null;
            // Releases the instance of the music
            _currentMusicInstance.release();
        }

        // Creates an instance for the audio
        _currentMusicInstance = CreateInstanceFromReference(reference);
        // Sets the initial volume to 0
        _currentMusicInstance.setVolume(0);
        // Starts playing the music
        _currentMusicInstance.start();

        // Starts the process of changing the volume from 0 to 1
        _musicFadeInCoroutine = StartCoroutine
            (ChangePersistentAudioVolume(_currentMusicInstance, 1,true));
        yield return _musicFadeInTime;
        _musicFadeInCoroutine = null;
        _musicStartCoroutine = null;
    }

    /// <summary>
    /// Changes the current music volume specifically
    /// </summary>
    /// <param name="endMusicVolume"> The end volume </param>
    public void ChangeCurrentMusicVolume(float endMusicVolume)
    {
        StartCoroutine(ChangePersistentAudioVolume(_currentMusicInstance, endMusicVolume,true));
    }
    #endregion

    #region Events
    public void InvokeOnBossMusicStarted()
    {
        _onBossMusicStarted?.Invoke();
    }

    public void InvokeOnBossMusicEnded()
    {
        _onBossMusicEnded?.Invoke();
    }
    #endregion

    #region Getters
    public UnityEvent GetOnBossMusicStartedEvent() => _onBossMusicStarted;
    public UnityEvent GetOnBossMusicEndedEvent() => _onBossMusicEnded;
    #endregion
}
