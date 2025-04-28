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

    // Cached variables
    private WaitForSeconds _animationWait;

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
        if (_objectiveHudAnimator.GetCurrentAnimatorStateInfo(0).IsName("ObjectiveSlideIn"))
        {
            _objectiveHudAnimator.SetTrigger("AbortSlide");
            _objectiveHudAnimator.ResetTrigger("SlideOut");
            StopCoroutine(WaitForAnimation());
        }

        foreach(Image h in _objectiveSprites)
        {
            h.enabled = false;
        }
        _objectiveHudText.enabled = false;

        //update text
        _objectiveHudText.text = objectiveHudTextString;
        // Mirrors info in pause menu
        SetPauseMenuObjective(objectiveHudTextString);
        //starts slide animation
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
        yield return new WaitForSeconds(0.5f);
        foreach (Image h in _objectiveSprites)
        {
            h.enabled = true;
        }
        _objectiveHudText.enabled = true;
        yield return _animationWait;
        DeactivateObjectiveHud();
    }
}
