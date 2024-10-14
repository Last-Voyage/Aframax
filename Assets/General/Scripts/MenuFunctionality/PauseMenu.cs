/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
//                     Ryan Swanson
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
    [SerializeField] private GameObject _pauseMenuContents;

    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => PauseToggle();
    }

    /// <summary>
    /// toggles the pause state so you can press escape again to close the pause menu
    /// </summary>
    private void PauseToggle()
    {
        TimeManager.Instance.PauseGameToggle();
    }

    /// <summary>
    /// Enables and disables the pause menu ui
    /// </summary>
    /// <param name="visible"></param>
    private void PauseUIVisibility(bool visible)
    {
        _pauseMenuContents.SetActive(false);
    }

    /// <summary>
    /// it quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();

        TimeManager.Instance.GetGamePauseToggleEvent().AddListener(PauseUIVisibility);
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();

        TimeManager.Instance.GetGamePauseToggleEvent().RemoveListener(PauseUIVisibility);
    }
}
