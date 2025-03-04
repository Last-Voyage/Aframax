/*****************************************************************************
// File Name :         ObjectiveHudBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     3/4/25
//
// Brief Description : handles hud and pause menu objective notifications
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveHudBehaviour : MonoBehaviour
{
    private GameObject _objectiveHudBackground;
    private GameObject _objectiveHudIcon;
    private Text _objectiveHudText;

    private Text _objectivePauseText;

    /// <summary>
    /// set references
    /// </summary>
    private void Awake()
    {
        //set references
    }

    /// <summary>
    /// run the objective animation for the player's current objective
    /// </summary>
    /// <param name="objectiveTextString"></param>
    private void activateObjectiveHud(string objectiveTextString) 
    {
        
    }

    /// <summary>
    /// set the objective text in the pause menu
    /// </summary>
    /// <param name="objectivePauseTextString"></param>
    private void setPauseMenuObjective(string objectivePauseTextString)
    {
        _objectivePauseText.text = objectivePauseTextString;
    }

    /// <summary>
    /// used for timing the wait between sliding in and out the objective hud
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitForAnimation()
    {
        yield return null;
    }
}
