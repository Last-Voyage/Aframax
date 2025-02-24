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

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
        SwapToSceneAudio();
    }

    /// <summary>
    /// Plays the audio associated with a scene
    /// </summary>
    private void SwapToSceneAudio()
    {
        PersistentAudioManager.Instance.StartMusicByID(_sceneMusicID);
    }
}
