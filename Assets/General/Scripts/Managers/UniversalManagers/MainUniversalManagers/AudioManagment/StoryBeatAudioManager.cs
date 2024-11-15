/******************************************************************************
// File Name:       StoryBeatAudioManager
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

public class AudioManagerTester : MonoBehaviour
{
    [SerializeField] public EventReference[] OneshotSounds;
    [SerializeField] public EventReference[] AmbientSounds;

    [SerializeField] private AudioManager _manager;
    private EventInstance _audioEvent;

    /// <summary>
    /// Used to play a shorter and non-persistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayOneshotSound(int index)
    {
        _manager.PlayOneShotSound(OneshotSounds[index]);
    }

    /// <summary>
    /// Used to play a spersistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayAmbient(int index)
    {
        _audioEvent = _manager.PlayAmbientSound(AmbientSounds[index]);
    }

    /// <summary>
    /// Used to stop the persistant sound
    /// </summary>
    public void StopAmbient()
    {
        _manager.StopAmbientSound(_audioEvent);
    }
}
