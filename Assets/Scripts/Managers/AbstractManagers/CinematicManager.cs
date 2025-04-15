/*****************************************************************************
// File Name :         CinematicManager.cs
// Author :            Charlie Polonus
// Contributor:        Jeremiah Peters
// Creation Date :     3/2/25
//
// Brief Description : Manages the different cinematics that play throughout
                       the game
*****************************************************************************/

using System.Collections;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System;

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

    [SerializeField] private TMP_Text _skipPromptText;

    [SerializeField] private Animator _skipPromptTextAnimator;

    [SerializeField] private int _skipPromptDuration;

    private PlayerInputMap _playerInputControls;

    private bool _skipTextActive;

    private void Awake()
    {
        _playerInputControls = new PlayerInputMap();

        //check for any input to show the skip cutscene text
        _playerInputControls.Player.SkipPrompt.started += StartShowSkipText;
    }

    /// <summary>
    /// Starts the cinematic
    /// </summary>
    private void Start()
    {
        _skipPromptTextAnimator = GetComponentInChildren<Animator>();
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
        // Stop all sounds
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/", out Bus masterBus);
        masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);

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
    /// Starts the process of showing the skip text
    /// </summary>
    /// <param name="ctx"> The input context </param>
    private void StartShowSkipText(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ShowSkipText(_skipPromptDuration));
    }

    /// <summary>
    /// makes the skip text appear on screen, then disappear after a while
    /// also enables and disables the input for skipping
    /// </summary>
    /// <param name="skipTimer">time the text remains on screen</param>
    /// <returns></returns>
    private IEnumerator ShowSkipText(int skipTimer)
    {
        if (_skipTextActive == false)
        {
            _skipTextActive = true;
            yield return null;
            _playerInputControls.Player.SkipCinematic.started += SkipCinematic;

            while (skipTimer >= 1)
            {
                skipTimer--;
                _skipPromptTextAnimator.SetInteger("FadeTimer", skipTimer);

                if (skipTimer == 0)
                {
                    //Debug.Log("hi");
                    _playerInputControls.Player.SkipCinematic.started -= SkipCinematic;
                    _skipTextActive = false;
                    break;
                }
                yield return null;
            }
        }
        yield return null;
    }

    /// <summary>
    /// calls LoadNextScene
    /// </summary>
    /// <param name="ctx">in order for the input to be disablable it needs to send the context</param>
    private void SkipCinematic(InputAction.CallbackContext ctx)
    {
        LoadNextScene();
    }

    /// <summary>
    /// Finish the cinematic, so load the next scene
    /// </summary>
    public void LoadNextScene()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(_sceneId, _sceneTransitionId);

        if (_cinematicAudio.isValid())
        {
            _cinematicAudio.stop(STOP_MODE.IMMEDIATE);
            _cinematicAudio.release();
        }
    }

    /// <summary>
    /// Peforms any needed functionality for when this is enabled
    /// </summary>
    private void OnEnable()
    {
        _playerInputControls.Enable();
    }

    /// <summary>
    /// Performs any needed functionality for when this is disabled
    /// </summary>
    private void OnDisable()
    {
        _playerInputControls.Player.SkipPrompt.started -= StartShowSkipText;
        _playerInputControls.Disable();
    }
}
