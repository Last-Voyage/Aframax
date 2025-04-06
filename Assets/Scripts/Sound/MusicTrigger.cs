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
    [SerializeField] private int _exitSoundId;
    [SerializeField] private int _enterSoundId;
    [SerializeField] private bool _canPlayOnEnter = false;
    [SerializeField] private bool _canPlayOnExit = false;
    private bool _playDoOnce = true;


    /// <summary>
    /// plays a sound on enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && _canPlayOnEnter && _playDoOnce)
        {
            EventReference[] sounds = FmodPersistentAudioEvents.Instance.MusicInGame;
            _eventInstance = PersistentAudioManager.Instance.CreateInstanceFromReference(sounds[_enterSoundId]);

            RuntimeManager.AttachInstanceToGameObject(_eventInstance,
                GameObject.FindGameObjectWithTag("Player").transform ,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>());

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
        if (other.gameObject.tag == "Player" && _canPlayOnExit && _playDoOnce)
        {
            EventReference[] sounds = FmodPersistentAudioEvents.Instance.MusicInGame;
            _eventInstance = PersistentAudioManager.Instance.CreateInstanceFromReference(sounds[_exitSoundId]);

            RuntimeManager.AttachInstanceToGameObject(_eventInstance,
                GameObject.FindGameObjectWithTag("Player").transform,
                GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>());

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
