/******************************************************************************
// File Name:       AudioManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster, David Henvick
// Creation Date:   September 14, 2024
//
// Description:     Provides the functionality behind all audio
                    Manager to be developed as I know specifics
******************************************************************************/
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

/// <summary>
/// Provides the functionality behind all audio
/// Manager to be developed
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    #region Variables
    public static AudioManager audioManager;
    Dictionary<EventReference, EventInstance> AudioPairs;

    private void Awake()
    {
        if (audioManager != null && audioManager != this)
            Destroy(audioManager.gameObject);

        audioManager = this;
        AudioPairs = new Dictionary<EventReference, EventInstance>();
    }
    #endregion Variables


    #region Base Manager

    public override void SetUpInstance()
    {
        base.SetUpInstance();
    }

    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
    }

    #endregion

    #region Getters

    #endregion

    #region Public Functions
    /// <summary>
    /// Plays a one shot sound that removes it's self after ending
    /// </summary>
    /// <param name="eventReference"></param> the sound you want played
    /// <param name="worldPosition"></param> the position you want it to be played
    public void PlayOneShotSFX(EventReference eventReference, Vector3 worldPosition = new Vector3())
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
    public EventInstance PlayAmbientSFX(EventReference reference)
    {
        if (reference.IsNull)
        {
            Debug.LogWarning("NO REFERENCE SOUND!");
            return default;
        }

        EventInstance audioEvent;

        if (AudioPairs.ContainsKey(reference))
            AudioPairs.TryGetValue(reference, out audioEvent);
        else
        {
            audioEvent = RuntimeManager.CreateInstance(reference);
            AudioPairs.Add(reference, audioEvent);
        }

        audioEvent.start();
        return audioEvent;
    }

    /// <summary>
    /// stops an FMOD sound using the reference it takes
    /// </summary>
    /// <param name="audioEvent">the sound instance you want ended</param>
    /// <param name="fade">toggles whether the sound will fade when done playing</param>
    public void StopAmbientSFX(EventInstance instance, bool fade = false)
    {
        if (instance.isValid())
        {
            instance.stop(fade ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
        }
        else
        {
            Debug.LogWarning("Null audioEvent. Please use a real event instance.");
        }
    }

    #endregion Public Function
}
