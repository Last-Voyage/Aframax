using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSound : AudioManager
{
    [field: SerializeField]
    public EventReference Sound;
    private EventInstance _eventInstance;

    /// <summary>
    /// happens whent he scene starts
    /// it triggers the fmode event
    /// </summary>
    private void Start()
    {
        _eventInstance = RuntimeManager.CreateInstance(Sound);
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
