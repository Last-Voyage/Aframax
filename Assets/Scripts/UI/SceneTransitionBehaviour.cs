/******************************************************************************
// File Name:       SceneTransitionBehaviour.cs
// Author:          Jeremiah Peters
// Contributor:     Ryan Swanson
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
}
