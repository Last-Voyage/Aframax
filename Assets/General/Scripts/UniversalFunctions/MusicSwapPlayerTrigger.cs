/*****************************************************************************
// File Name :         MusicSwapPlayerTrigger.cs
// Author :            Ryan Swanson
// Creation Date :     02/24/2025
//
// Brief Description : A trigger that swaps either the volume or current music playing when touched by the player
*****************************************************************************/

using UnityEngine;

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

    [field: SerializeField] private bool _destroyOnContact;

    private void Start()
    {
        transform.SetParent(null);
    }


    /// <summary>
    /// Called when the player contacts this
    /// </summary>
    public void PlayerContact()
    {
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
}
