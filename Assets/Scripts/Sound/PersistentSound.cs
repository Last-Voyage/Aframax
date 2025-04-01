
/******************************************************************************
// File Name:       PersistentSound.cs
// Author:          Nabil Tagba
// Creation Date:   March 30, 2025
//
// Description:     plays an fmod event persistently
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
    [SerializeField] private bool _shouldPlay = true;
    /// <summary>
    /// happens when the scene starts
    /// it triggers the fmod event
    /// </summary>
    private void Start()
    {
        if (_shouldPlay)
        {
            EventReference[] sounds = FmodPersistentAudioEvents.Instance.PersistentSound;
            _eventInstance = PersistentAudioManager.Instance.CreateInstanceFromReference(sounds[_soundId], this.gameObject); ;
            _eventInstance.getDescription(out EventDescription eventDesc);
            eventDesc.isOneshot(out bool isOneShot);
            if (isOneShot) 
            { 
                Debug.LogWarning("The event is a one shot, go in fmod and add loop region");
            }
            _eventInstance.start();
        }
       
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
