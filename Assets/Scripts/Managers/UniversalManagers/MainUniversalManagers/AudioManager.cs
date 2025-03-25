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
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Provides the base functionality for all audio managers
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
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
}
