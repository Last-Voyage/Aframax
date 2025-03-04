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
    [SerializeField] private float _objectiveLingerTime;
    private Animator _objectiveHudAnimator;
    private Text _objectiveHudText;
    private Text _objectivePauseText;

    /// <summary>
    /// set references
    /// </summary>
    private void Awake()
    {
        //set references
        _objectiveHudAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// slides the objective onto screen with the corresponding text
    /// </summary>
    /// <param name="objectiveHudTextString"></param>
    private void activateObjectiveHud(string objectiveHudTextString) 
    {
        _objectiveHudText.text = objectiveHudTextString;
        _objectiveHudAnimator.SetTrigger("SlideIn");
        StartCoroutine(waitForAnimation());
    }

    /// <summary>
    /// slides the objective back off screen
    /// </summary>
    private void deactivateObjectiveHud()
    {
        _objectiveHudAnimator.SetTrigger("SlideOut");
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
        yield return new WaitForSeconds(_objectiveLingerTime);
        deactivateObjectiveHud();
    }
}
