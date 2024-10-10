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
    [SerializeField] private List<GameObject> PauseMenuContents;

    PlayerInputMap Controls;
    private bool Pause = false;

    private void Awake()
    {
        //initialize input
        Controls = new PlayerInputMap();
        Controls.Player.Pause.performed += ctx => PauseToggle();

        //find the pause menu objects
        foreach (Transform child in gameObject.transform)
        {
            //this is the thing that breaks when PauseMenuContents isn't serialized
            PauseMenuContents.Add(child.gameObject);
        }
    }

    /// <summary>
    /// toggles the pause state so you can press escape again to close the pause menu
    /// </summary>
    private void PauseToggle()
    {
        Pause = !Pause;
        if (Pause)
        {
            PauseGame();
        }
        else if (!Pause)
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
            //this should be hooked up to the time manager, but the manager is empty rn so idk
            Time.timeScale = 0;

            //turn on the pause menu stuff
            foreach (GameObject menuContents in PauseMenuContents)
            {
                menuContents.SetActive(true);
            }
        }
    }

    /// <summary>
    /// unpauses the game by setting the timeScale to 1 and deactivates the pause menu objects
    /// </summary>
    public void ResumeGame()
    {
        //this should be hooked up to the time manager, but the manager is empty rn so idk
        Time.timeScale = 1;

        //turn off the pause menu stuff
        foreach (GameObject menuContents in PauseMenuContents)
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

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }
}
