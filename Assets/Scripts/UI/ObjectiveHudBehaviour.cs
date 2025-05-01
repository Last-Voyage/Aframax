/*****************************************************************************
// File Name :         ObjectiveHudBehaviour.cs
// Author :            Jeremiah Peters
// Contributors :      Adam Garwacki
// Creation Date :     3/4/25
//
// Brief Description : handles hud and pause menu objective notifications
*****************************************************************************/

using System;
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
    [SerializeField] private Image[] _objectiveSprites;
    private Coroutine _animatorWaitTime;

    // Cached variables
    private WaitForSeconds _animationWait;

    //const animator info strings
    private const string _slideInTrigger = "SlideIn";
    private const string _slideOutTrigger = "SlideOut";
    private const string _slideAbortTrigger = "AbortSlide";
    private const string _slideInAnimState = "ObjectiveSlideIn";
    private const string _slideOutAnimState = "ObjectiveSlideOut";

    /// <summary>
    /// This initializes the cached variable
    /// </summary>
    private void Awake()
    {
        _animationWait = new WaitForSeconds(_objectiveLingerTime);
        _objectiveSprites = GetComponentsInChildren<Image>();
    }

    /// <summary>
    /// slides the objective onto screen with the corresponding text
    /// </summary>
    /// <param name="objectiveHudTextString">the text that goes on the ui</param>
    public void ActivateObjectiveHud(string objectiveHudTextString) 
    {
        //reset to default state if the animation is already going
        if (_objectiveHudAnimator.GetCurrentAnimatorStateInfo(0).IsName(_slideInAnimState) || 
            _objectiveHudAnimator.GetCurrentAnimatorStateInfo(0).IsName(_slideOutAnimState))
        {
            _objectiveHudAnimator.SetTrigger(_slideAbortTrigger);
            _objectiveHudAnimator.ResetTrigger(_slideInTrigger);
            _objectiveHudAnimator.ResetTrigger(_slideOutTrigger);
            StopCoroutine(_animatorWaitTime);
        }


        foreach(Image objVisual in _objectiveSprites)
        {
            objVisual.enabled = false;
        }
        _objectiveHudText.enabled = false;

        //update text
        _objectiveHudText.text = objectiveHudTextString;
        // Mirrors info in pause menu
        SetPauseMenuObjective(objectiveHudTextString);
        //starts slide animation
        _objectiveHudAnimator.SetTrigger(_slideInTrigger);
        _animatorWaitTime = StartCoroutine(WaitForAnimation());
    }

    /// <summary>
    /// slides the objective back off screen
    /// </summary>
    private void DeactivateObjectiveHud()
    {
        _objectiveHudAnimator.SetTrigger(_slideOutTrigger);
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
        yield return new WaitForSeconds(0.5f);
        foreach (Image objVisual in _objectiveSprites)
        {
            objVisual.enabled = true;
        }
        _objectiveHudAnimator.ResetTrigger(_slideInTrigger);
        _objectiveHudText.enabled = true;
        yield return _animationWait;
        DeactivateObjectiveHud();
    }
}
