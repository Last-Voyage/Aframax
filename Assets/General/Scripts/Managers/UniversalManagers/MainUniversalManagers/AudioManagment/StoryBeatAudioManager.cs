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

    [SerializeField] AudioManager Manager;
    EventInstance _audioEvent;

    /// <summary>
    /// Used to play a shorter and non-persistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayOneshotSound(int index)
    {
        Manager.PlayOneShotSound(OneshotSounds[index]);
    }

    /// <summary>
    /// Used to play a spersistant sound
    /// </summary>
    /// <param name="index"></param> will be the index of the desired sound
    public void PlayAmbient(int index)
    {
        _audioEvent = Manager.PlayAmbientSound(AmbientSounds[index]);
    }

    /// <summary>
    /// Used to stop the persistant sound
    /// </summary>
    public void StopAmbient()
    {
        Manager.StopAmbientSound(_audioEvent);
    }
}
