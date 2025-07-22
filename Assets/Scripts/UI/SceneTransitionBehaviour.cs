/******************************************************************************
// File Name:       SceneTransitionBehaviour.cs
// Author:          Jeremiah Peters
// Contributors:    Ryan Swanson, Adam Garwacki
// Creation Date:   February 6, 2025
//
// Description:     provides functionality for scene transitions
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// plays scene transition animations
/// </summary>
public class SceneTransitionBehaviour : MonoBehaviour
{
    private Animator _sceneTransitionAnimator;
    private Image _transitionImage;

    public static SceneTransitionBehaviour Instance;

    /// <summary>
    /// sets up references
    /// </summary>
    public void Setup()
    {
        _sceneTransitionAnimator = GetComponent<Animator>();
        _transitionImage = GetComponent<Image>();
        Instance = this;
    }

    /// <summary>
    /// plays scene transition
    /// </summary>
    /// <param name="animationTrigger"></param>
    public void PlayTransition(string animationTrigger)
    {
        _transitionImage.enabled = true;
        _sceneTransitionAnimator.SetTrigger(animationTrigger);
    }

    /// <summary>
    /// Checks whether a transition is currently in play.
    /// </summary>
    public bool CheckIfTransitionIsActive()
    {
        return _sceneTransitionAnimator.enabled;
    }
 

    /// <summary>
    /// Prematurely ends the transition currently in use.
    /// </summary>
    public void DisableTransition()
    {
        _transitionImage.enabled = false;
    }
}
