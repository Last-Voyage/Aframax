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
    private List<GameObject> _pauseMenuContents = new();

    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => PauseToggle();

        //find the pause menu objects
        foreach (Transform child in gameObject.transform)
        {
            _pauseMenuContents.Add(child.gameObject);
        }
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
        //Debug.Log(menuContents.gameObject.activeSelf);
        foreach (GameObject menuContents in _pauseMenuContents)
        {
            menuContents.SetActive(!menuContents.gameObject.activeSelf);
        }
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
