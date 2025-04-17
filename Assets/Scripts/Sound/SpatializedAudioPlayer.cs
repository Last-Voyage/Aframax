/*****************************************************************************
// File Name :         SpatializedAudioPlayer.cs
// Author :            Charlie Polonus
// Creation Date :     4/15/2025
//
// Brief Description : Allows the playing of spatialized audio through the
                       story manager
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows playing spatialized audio through the story manager
/// </summary>
public class SpatializedAudioPlayer : MonoBehaviour
{
    private StoryBeatAudioManager _storyBeatAudioManager;

    /// <summary>
    /// Finds the audio manager in the scene
    /// </summary>
    private void Start()
    {
        _storyBeatAudioManager = FindObjectOfType<StoryBeatAudioManager>();
    }

    /// <summary>
    /// Plays the indexed oneshot sound
    /// </summary>
    /// <param name="index">The index of the sound to play</param>
    public void PlaySpatializedOneShot(int index)
    {
        _storyBeatAudioManager.PlayOneshotSound(index);
    }
}
