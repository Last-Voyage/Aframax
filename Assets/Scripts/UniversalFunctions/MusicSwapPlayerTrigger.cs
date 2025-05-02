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
    None,
    SwapMusic,
    SwapVolume
}

/// <summary>
/// Trigger for changing some aspects of the music
/// </summary>
public class MusicSwapPlayerTrigger : MonoBehaviour
{
    [Header("Enter")]
    [Tooltip("What we want to happen on player contact")]
    [field: SerializeField] private EMusicTriggerTypes _contactType;

    [Tooltip("The ID of the music to play. Check FmodPersistentAudioEvents for the specific IDs")]
    [field: SerializeField] private int _musicEnterID;

    [Tooltip("The volume to switch to")]
    [field: SerializeField] [Range(0,1)] private float _newEnterVolume;

    [field: SerializeField] private UnityEvent _onPlayerContact;

    [Header("Exit")]
    [field: SerializeField] private EMusicTriggerTypes _exitType;

    [field: SerializeField] private int _musicExitID;

    [field: SerializeField][Range(0, 1)] private float _newExitVolume;

    [field: SerializeField] private UnityEvent _onPlayerExit;

    [Header("General")]
    [field: SerializeField] private bool _detachOnStart = true;
    [field: SerializeField] private bool _destroyOnContact;

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

        PlayerCollision(true, _contactType);

        if(_destroyOnContact)
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Called when the player exits collision
    /// </summary>
    public void PlayerExit()
    {
        _onPlayerExit?.Invoke();

        PlayerCollision(false, _exitType);
    }

    /// <summary>
    /// Called to determine what to do on player collision
    /// </summary>
    /// <param name="isEnter">If the collision came from entering contact</param>
    /// <param name="triggerType"> The type of action to take from contact </param>
    private void PlayerCollision(bool isEnter, EMusicTriggerTypes triggerType)
    {
        if (triggerType == EMusicTriggerTypes.SwapMusic)
        {
            int musicID = isEnter ? _musicEnterID : _musicExitID;
            SwapMusic(musicID);
        }
        else if (triggerType == EMusicTriggerTypes.SwapVolume)
        {
            float volume = isEnter ? _newEnterVolume : _newExitVolume;
            //Using an Else If just in case we end up adding more EMusicTriggerTypes
            SwapVolume(volume);
        }
    }

    /// <summary>
    /// Switches the music to play
    /// </summary>
    private void SwapMusic(int id)
    {
        PersistentAudioManager.Instance.StartMusicByID(id);
    }

    /// <summary>
    /// Switches the volume of the music
    /// </summary>
    private void SwapVolume(float volume)
    {
        PersistentAudioManager.Instance.ChangeCurrentMusicVolume(_newEnterVolume);
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

    /// <summary>
    /// Remove all listeners on destruction
    /// </summary>
    private void OnDestroy()
    {
        _onPlayerContact?.RemoveAllListeners();
        _onPlayerExit?.RemoveAllListeners();
    }
}
