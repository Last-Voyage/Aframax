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

public class SceneTransitionBehaviour : MonoBehaviour
{
    private Animator _sceneTransitionAnimator;

    public static SceneTransitionBehaviour Instance;

    /// <summary>
    /// sets up references
    /// </summary>
    public void Setup()
    {
        _sceneTransitionAnimator = GetComponent<Animator>();
        Instance = this;
    }

    /// <summary>
    /// plays scene transition
    /// </summary>
    /// <param name="animationTrigger"></param>
    public void PlayTransition(string animationTrigger)
    {
        _sceneTransitionAnimator.SetTrigger(animationTrigger);
    }
}
