/******************************************************************************
// File Name:       AudioManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster, David Henvick
// Creation Date:   September 14, 2024
//
// Description:     Provides the base functionality for all audio managers
******************************************************************************/

using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides the base functionality for all audio managers
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    public static AudioManager Instance;

    /// <summary>
    /// Establishes the instance for the audio manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Creates an Fmod instance after being given an Fmod reference
    /// </summary>
    /// <param name="eventReference"> The reference to create the instance from</param>
    /// <returns></returns>
    public virtual EventInstance CreateInstanceFromReference(EventReference eventReference)
    {
        if (eventReference.IsNull)
        {
            return new EventInstance();
        }
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        return eventInstance;
    }

    /// <summary>
    /// Creates a Fmod instance after being given an Fmod reference and object to attach to
    /// </summary>
    /// <param name="eventReference"> The reference to create the instance from </param>
    /// <param name="attachedObject"> What the instance is attached to </param>
    /// <returns> The instance that was created </returns>
    public virtual EventInstance CreateInstanceFromReference(EventReference eventReference, GameObject attachedObject)
    {
        if (eventReference.IsNull)
        {
            return new EventInstance();
        }
        EventInstance eventInstance = CreateInstanceFromReference(eventReference);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(attachedObject));

        return eventInstance;
    }

    /// <summary>
    /// Fades in a sfx using an instance
    /// </summary>
    /// <param name="eventInstance"> The sfx to fade in </param>
    /// <param name="fadeTime"> The time to fade in </param>
    public virtual void FadeInLoopingOneShot(EventInstance eventInstance, float fadeTime)
    {
        eventInstance.start();

        StartCoroutine(FadeEventInstance(eventInstance, fadeTime, 1));
    }

    /// <summary>
    /// Fades out a sfx using an instance
    /// </summary>
    /// <param name="inst"> The sfx to fade out </param>
    /// <param name="fadeTime"> The time to fade out </param>
    public virtual void FadeOutLoopingOneShot(EventInstance inst, float fadeTime)
    {
        StartCoroutine(FadeEventInstance(inst, fadeTime, 0));
    }

    /// <summary>
    /// Moves an instances volume from where it starts to an end
    /// </summary>
    /// <param name="eventInstance"> The instance to adjust the volume </param>
    /// <param name="fadeTime"> The time to fade </param>
    /// <param name="endVol"> The ending volume </param>
    /// <returns></returns>
    protected virtual IEnumerator FadeEventInstance(EventInstance eventInstance, float fadeTime, float endVol)
    {
        float progress = 0;
        float currentVol;
        eventInstance.getVolume(out float startingVol);
        while (progress < 1)
        {
            progress += Time.deltaTime / fadeTime;

            currentVol = Mathf.Lerp(startingVol, endVol, progress);
            currentVol = Mathf.Clamp(currentVol, 0, 1);

            eventInstance.setVolume(currentVol);
            yield return null;
        }
    }
}
