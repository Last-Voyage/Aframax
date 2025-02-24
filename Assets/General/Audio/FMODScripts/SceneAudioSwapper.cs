/*****************************************************************************
// File Name :         SceneAudioSwapper.cs
// Author :            Ryan Swanson
// Creation Date :     02/24/2025
//
// Brief Description : Switches the audio on scene switch
*****************************************************************************/

using UnityEngine;

public class SceneAudioSwapper : MonoBehaviour
{
    [Tooltip("The ID of the music to play. Check FmodPersistentAudioEvents for the specific IDs")]
    [SerializeField] private int _sceneMusicID;

    /// <summary>
    /// Plays the audio associated with a scene
    /// </summary>
    public void SwapToSceneAudio()
    {
        transform.parent = null;
        PersistentAudioManager.Instance.StartMusicByID(_sceneMusicID);
        Destroy(gameObject);
    }
}
