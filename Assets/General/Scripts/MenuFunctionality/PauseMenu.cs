/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
// Creation Date :     9/28/24
//
// Brief Description : operates pausing the game and the pause menu buttons
*****************************************************************************/
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// functionality for pausing the game and the pause menu buttons
/// </summary>
public class PauseMenu : MonoBehaviour
{
    //for some ungodly reason, this script only works when this is serialized or public
    [SerializeField] private List<GameObject> _pauseMenuContents;

    private PlayerInputMap _playerInputControls;
    private bool _isPaused = false;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => PauseToggle();

        //find the pause menu objects
        foreach (Transform child in gameObject.transform)
        {
            //this is the thing that breaks when PauseMenuContents isn't serialized
            _pauseMenuContents.Add(child.gameObject);
        }
    }

    /// <summary>
    /// toggles the pause state so you can press escape again to close the pause menu
    /// </summary>
    private void PauseToggle()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            PauseGame();
        }
        else if (!_isPaused)
        {
            ResumeGame();
        }
    }

    /// <summary>
    /// pauses the game by setting the timeScale to 0 and activates the pause menu objects
    /// </summary>
    public void PauseGame()
    {
        //don't pause if the game is already paused
        if (Time.timeScale > 0)
        {
            TimeManager.Instance.InvokeOnGamePause(true);
            CameraManager.Instance.InvokeOnCameraMovementToggle(false);

            //turn on the pause menu stuff
            foreach (GameObject menuContents in _pauseMenuContents)
            {
                menuContents.SetActive(true);
            }
        }
    }

    /// <summary>
    /// un-pauses the game by setting the timeScale to 1 and deactivates the pause menu objects
    /// </summary>
    public void ResumeGame()
    {
        TimeManager.Instance.InvokeOnGamePause(false);
        CameraManager.Instance.InvokeOnCameraMovementToggle(true);

        //turn off the pause menu stuff
        foreach (GameObject menuContents in _pauseMenuContents)
        {
            menuContents.SetActive(false);
        }
    }

    /// <summary>
    /// it quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetTimeScale(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();

        TimeManager.Instance.GetGamePauseEvent().AddListener(SetTimeScale);
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();

        TimeManager.Instance.GetGamePauseEvent().RemoveListener(SetTimeScale);
    }
}
