/*****************************************************************************
// File Name :         CinematicManager.cs
// Author :            Charlie Polonus
// Creation Date :     3/2/25
//
// Brief Description : Manages the different cinematics that play throughout
                       the game
*****************************************************************************/

using System.Collections;

using FMOD.Studio;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Manages all the cinematics that play
/// </summary>
public class CinematicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Canvas _cinematicCanvas;

    [Header("Settings")]
    [SerializeField] private int _sceneId;
    [SerializeField] private int _sceneTransitionId;
    [Tooltip("The ID of the cinematic audio to play from the FmodSfxEvents under the universal managers")]
    [SerializeField] private int _cinematicAudioID;
    private bool _cinematicPlaying;

    private EventInstance _cinematicAudio;

    /// <summary>
    /// Starts the cinematic
    /// </summary>
    private void Start()
    {
        StartVideo();
        StartCinematicAudio();
    }

    /// <summary>
    /// Checks to see if the video is done and loads the next scene
    /// </summary>
    void Update()
    {
        // Is the cinematic playing, has it started, and is it far enough through that it doesn't immediately stop?
        if (!_videoPlayer.isPlaying && _cinematicPlaying && _videoPlayer.time > _videoPlayer.clip.length / 2f)
        {
            LoadNextScene();
        }
    }

    /// <summary>
    /// Plays the video, making sure everything is correct to reset the video
    /// </summary>
    private void StartVideo()
    {
        _cinematicPlaying = true;
        _videoPlayer.time = 0;
        _videoPlayer.Play();
    }

    /// <summary>
    /// Plays the audio for the cinematic
    /// </summary>
    private void StartCinematicAudio()
    {
        _cinematicAudio = RuntimeSfxManager.Instance.CreateInstanceFromReference
            (FmodSfxEvents.Instance.CinematicArray[_cinematicAudioID]);

        if(_cinematicAudio.isValid())
        {
            _cinematicAudio.start();
        }
    }

    /// <summary>
    /// Finish the cinematic, so load the next scene
    /// </summary>
    private void LoadNextScene()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(_sceneId, _sceneTransitionId);

        if (_cinematicAudio.isValid())
        {
            _cinematicAudio.stop(STOP_MODE.IMMEDIATE);
            _cinematicAudio.release();
        }
    }
}
