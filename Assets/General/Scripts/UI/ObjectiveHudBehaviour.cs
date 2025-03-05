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
using TMPro;

/// <summary>
/// runs the objective text for hud and pause menu as well as triggering the animation for the hud
/// </summary>
public class ObjectiveHudBehaviour : MonoBehaviour
{
    [SerializeField] private float _objectiveLingerTime;
    [SerializeField] private Animator _objectiveHudAnimator;
    [SerializeField] private TextMeshProUGUI _objectiveHudText;
    [SerializeField] private TextMeshProUGUI _objectivePauseText;

    /// <summary>
    /// slides the objective onto screen with the corresponding text
    /// </summary>
    /// <param name="objectiveHudTextString">the text that goes on the ui</param>
    public void ActivateObjectiveHud(string objectiveHudTextString) 
    {
        _objectiveHudText.text = objectiveHudTextString;
        _objectiveHudAnimator.SetTrigger("SlideIn");
        StartCoroutine(WaitForAnimation());
    }

    /// <summary>
    /// slides the objective back off screen
    /// </summary>
    private void DeactivateObjectiveHud()
    {
        _objectiveHudAnimator.SetTrigger("SlideOut");
    }

    /// <summary>
    /// set the objective text in the pause menu
    /// </summary>
    /// <param name="objectivePauseTextString">the text that goes on the pause menu</param>
    public void SetPauseMenuObjective(string objectivePauseTextString)
    {
        _objectivePauseText.text = objectivePauseTextString;
    }

    /// <summary>
    /// used for timing the wait between sliding in and out the objective hud
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(_objectiveLingerTime);
        DeactivateObjectiveHud();
    }
}
