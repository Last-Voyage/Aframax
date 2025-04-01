
/******************************************************************************
// File Name:       PersistentSound.cs
// Author:          Nabil Tagba
// Creation Date:   March 30, 2025
//
// Description:     plays an fmode event persistently
******************************************************************************/
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// plays a specific fmod event persistently
/// </summary>
public class PersistentSound : AudioManager
{
    
    private EventInstance _eventInstance;
    [SerializeField] private int _soundId;
    /// <summary>
    /// happens whent he scene starts
    /// it triggers the fmode event
    /// </summary>
    private void Start()
    {
        EventReference[] sounds = FmodPersistentAudioEvents.Instance.PersistentSound;
        _eventInstance = RuntimeManager.CreateInstance(sounds[_soundId]);
        _eventInstance.getDescription(out EventDescription eventDesc);
        eventDesc.isOneshot(out bool isOneShot);
        if (isOneShot) Debug.LogWarning("The event is a one shot, go in fmode and add loop region");
        _eventInstance.start();
       
    }


    /// <summary>
    /// happens when the object is destroyed
    /// stops the fmod event
    /// </summary>
    void OnDestroy()
    {
        // Stop the event when the object is destroyed
        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _eventInstance.release();
    }

}
