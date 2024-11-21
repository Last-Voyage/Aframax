/******************************************************************************
// File Name:       StoryBeatAudioManager.cs
// Author:          David Henvick
// Contributors:    
// Creation Date:   November 14, 2024
//
// Description:     Provides the public functions to play sounds
******************************************************************************/
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class StoryBeatAudioManager : MonoBehaviour
{
    [SerializeField] public EventReference[] OneshotSounds;
    [SerializeField] public EventReference[] AmbientSounds;
    
    private EventInstance _audioEvent;
    
    //dictionary to hold the currently playing to keep the same instance being layered on itself.
    private Dictionary<EventReference, EventInstance> _audioPairs;

    private void Start()
    {
        _audioPairs = new Dictionary<EventReference, EventInstance>();
    }
    
    /// <summary>
    /// Used to play a shorter and non-persistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayOneshotSound(int index)
    {
        StoryBeatOneShotSfx(OneshotSounds[index]);
    }

    /// <summary>
    /// Used to play a spersistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayAmbient(int index)
    {
        _audioEvent = StoryBeatPlayAmbientSound(AmbientSounds[index]);
    }

    /// <summary>
    /// Used to stop the persistant sound
    /// </summary>
    public void StopAmbient()
    {
        StoryBeatStopAmbientSound(_audioEvent);
    }
    
    /// <summary>
    /// Plays a one shot sound that removes it's self after ending
    /// </summary>
    /// <param name="eventReference"></param> the sound you want played
    /// <param name="worldPosition"></param> the position you want it to be played
    public void StoryBeatOneShotSfx(EventReference eventReference, Vector3 worldPosition = new Vector3())
    {
        if (eventReference.IsNull)
        {
            Debug.LogWarning(eventReference + " is null.");
            return;
        }

        RuntimeManager.PlayOneShot(eventReference, worldPosition);
    }
    
    /// <summary>
    /// Plays an FMOD sound using a reference. Just add the event reference and setup the sound in the dictionary
    /// </summary>
    /// <param name="reference">the sound reference you want to play</param>
    /// <returns>an EventInstance, save it if you need to use a parameter</returns>
    public EventInstance StoryBeatPlayAmbientSound(EventReference reference)
    {
        if (reference.IsNull)
        {
            Debug.LogWarning("NO REFERENCE SOUND!");
            return default;
        }

        EventInstance audioEvent;

        if (_audioPairs.ContainsKey(reference))
        {
            _audioPairs.TryGetValue(reference, out audioEvent);
        }

        else
        {
            audioEvent = RuntimeManager.CreateInstance(reference);
            _audioPairs.Add(reference, audioEvent);
        }

        audioEvent.start();
        return audioEvent;
    }
    
    /// <summary>
    /// stops an FMOD sound using the reference it takes
    /// </summary>
    /// <param name="audioEvent">the sound instance you want ended</param>
    /// <param name="fade">toggles whether the sound will fade when done playing</param>
    public void StoryBeatStopAmbientSound(EventInstance instance, bool willFade = false)
    {
        if (instance.isValid())
        {
            instance.stop(willFade ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
        }
        else
        {
            Debug.LogWarning("Null audioEvent. Please use a real event instance.");
        }
    }
}
