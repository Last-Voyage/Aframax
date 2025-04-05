/******************************************************************************
// File Name:       MusicTrigger.cs
// Author:          Nabil Tagba
// Creation Date:   April 4, 2025
//
// Description:     plays a sound on enter or exit
******************************************************************************/
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// plays a sound on enter or exit
/// </summary>
public class MusicTrigger : MonoBehaviour
{
    private EventInstance _eventInstance;
    [SerializeField] private int _soundId;
    [SerializeField] private bool _playOnEnter = false;
    [SerializeField] private bool _playOnExit = false;
    private bool _playDoOnce = true;


    /// <summary>
    /// plays a sound on enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player" && _playOnEnter && _playDoOnce)
        {
            EventReference[] sounds = FmodPersistentAudioEvents.Instance.MusicInGame;
            _eventInstance = PersistentAudioManager.Instance.CreateInstanceFromReference(sounds[_soundId],
                GameObject.FindGameObjectWithTag("Player"));
            _eventInstance.start();
            _playDoOnce = false;
        }
    }
    /// <summary>
    /// plays a sound on exit
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.tag == "Player" && _playOnExit && _playDoOnce)
        {
            EventReference[] sounds = FmodPersistentAudioEvents.Instance.MusicInGame;
            _eventInstance = PersistentAudioManager.Instance.CreateInstanceFromReference(sounds[_soundId],
                GameObject.FindGameObjectWithTag("Player"));
            _eventInstance.start();
            _playDoOnce = false;
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
