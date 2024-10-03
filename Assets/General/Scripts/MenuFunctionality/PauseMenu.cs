/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
// Creation Date :     9/28/24
//
// Brief Description : operates pausing the game and the pause menu buttons
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    //for some ungodly reason, this script only works when this is serialized or public
    [SerializeField] private List<GameObject> PauseMenuContents;

    private void Awake()
    {
        //find the pause menu objects
        foreach (Transform child in gameObject.transform)
        {
            //this is the thing that breaks when PauseMenuContents isn't serialized
            PauseMenuContents.Add(child.gameObject);
        }
    }

    private void Update()
    {
        //this should be hooked up to the new input system
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
        }
    }

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

    public void QuitGame()
    {
        Application.Quit();
    }
}
