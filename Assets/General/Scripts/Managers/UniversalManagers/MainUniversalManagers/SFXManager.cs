/******************************************************************************
// File Name:       SFXManager.cs
// Author:          Andrea Swihart-DeCoster
// Creation Date:   October 1st, 2024
//
// Description:     Manages one shot sound effects.
******************************************************************************/
using FMODUnity;
using System;
using UnityEngine;

public class SFXManager : AudioManager
{
    public static Action<EventReference> APlayOneShotSFX;

    #region Enable and Action Subscriptions
    private void OnEnable()
    {
        SubscribeToActions(true);
    }
    
    private void OnDisable()
    {
        SubscribeToActions(false);
    }

    /// <summary>
    /// Subscribes and unsubscribes from actions
    /// </summary>
    /// <param name="val">if true, subscribes</param>
    private void SubscribeToActions(bool val)
    {
        if(val)
        {
            APlayOneShotSFX += PlayOneShotSFX;
            return;
        }

        APlayOneShotSFX -= PlayOneShotSFX;
    }
    #endregion Enable and Action Subscriptions

    #region FMOD Audio Functionality

    /// <summary>
    /// Plays an audio event at a specific position
    /// </summary>
    /// <param name="eventReference">reference to the FMOD SFX event </param>
    private void PlayOneShotSFX(EventReference eventReference)
    {
        if(eventReference.IsNull)
        {
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot(eventReference);
    }
    #endregion
}
