/*****************************************************************************
// File Name :         MusicSwapPlayerTrigger.cs
// Author :            Ryan Swanson
// Creation Date :     02/24/2025
//
// Brief Description : A trigger that swaps either the volume or current music playing when touched by the player
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The types of actions to take on player contact
/// </summary>
public enum EMusicTriggerTypes
{
    SwapMusic,
    SwapVolume
}

/// <summary>
/// Trigger for changing some aspects of the music
/// </summary>
public class MusicSwapPlayerTrigger : MonoBehaviour
{
    [Tooltip("What we want to happen on player contact")]
    [field: SerializeField] private EMusicTriggerTypes _contactType;

    [Tooltip("The ID of the music to play. Check FmodPersistentAudioEvents for the specific IDs")]
    [field: SerializeField] private int _musicID;

    [Tooltip("The volume to switch to")]
    [field: SerializeField] [Range(0,1)] private float _newVolume;

    [field: SerializeField] private bool _detachOnStart = true;
    [field: SerializeField] private bool _destroyOnContact;

    [field: SerializeField] private UnityEvent _onPlayerContact;

    /// <summary>
    /// Removes the parent associate with this. That way it can be safely attached to other prefabs.
    /// </summary>
    private void Start()
    {
        if(_detachOnStart)
        {
            transform.SetParent(null);
        }
    }

    /// <summary>
    /// Called when the player contacts this
    /// </summary>
    public void PlayerContact()
    {
        _onPlayerContact?.Invoke();
        if(_contactType == EMusicTriggerTypes.SwapMusic)
        {
            SwapMusic();
        }
        else if(_contactType == EMusicTriggerTypes.SwapVolume)
        {
            //Using an Else If just in case we end up adding more EMusicTriggerTypes
            SwapVolume();
        }

        if(_destroyOnContact)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Switches the music to play
    /// </summary>
    private void SwapMusic()
    {
        PersistentAudioManager.Instance.StartMusicByID(_musicID);
    }

    /// <summary>
    /// Switches the volume of the music
    /// </summary>
    private void SwapVolume()
    {
        PersistentAudioManager.Instance.ChangeCurrentMusicVolume(_newVolume);
    }

    /// <summary>
    /// Called when boss music is activated to invoke the events
    /// </summary>
    /// <param name="active"> If it is active or not </param>
    public void BossMusicActivation(bool active)
    {
        if(active)
        {
            PersistentAudioManager.Instance.InvokeOnBossMusicStarted();
        }
        else
        {
            PersistentAudioManager.Instance.InvokeOnBossMusicEnded();
        }
    }
}
