/*****************************************************************************
// File Name :         BossAttackActSystem.cs
// Author :            Mark Hanson
// Creation Date :     10/22/2024
//
// Brief Description : The system to manage what act the boss is on and also switch between them along with which attack comes out
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Defines an act
/// </summary>
[System.Serializable]
public struct Act
{
    [field: Tooltip("Scenes within the act")]
    [field: SerializeField] public ActScene[] Scenes { get; private set; }
}

/// <summary>
/// Defines a scene within an act
/// </summary>
[System.Serializable]
public struct ActScene
{
    [field: Tooltip("Attacks within the Act")]
    [field: SerializeField] public BaseBossAttack[] SceneAttacks { get; private set; }
}

/// <summary>
/// A class that contains multiple functions for the act system that are updated within a coroutine
/// </summary>
public class BossAttackActSystem : MonoBehaviour
{
    #region Act Variables

    [Tooltip("Acts within the boss fight")]
    [SerializeField] private Act[] _bossFightActs;

    /// <summary>
    /// Current act position in array
    /// </summary>
    private int _currentActNum;
    /// <summary>
    /// Current act ref
    /// </summary>
    private Act _currentAct;

    [Tooltip("Invoked when an act begins")]
    private UnityEvent _actBegin = new();

    [Tooltip("Invoked when an act ends")]
    private UnityEvent _actEnd = new();

    #endregion Act Variables

    #region Act Scene Variables

    /// <summary>
    /// Current scene position in arr
    /// </summary>
    private int _currentSceneNum;
    /// <summary>
    /// Current scene ref
    /// </summary>
    private ActScene _currentScene;

    [Tooltip("# of attacks completed")]
    protected private int _attackCounter = 0;

    [Tooltip("Invoked when a scene begins")]
    private UnityEvent _sceneBegin = new();

    [Tooltip("Invoked when a scene ends")]
    private UnityEvent _sceneEnd = new();

    #endregion

    private void Start()
    {
        InitializeActVariables();
        InitializeSceneVariables();
    }

    /// <summary>
    /// just for testing
    /// </summary>
    private void Update()
    {
        // TODO - Connect this to the end of the tutorial
        // Test the begin interior attack until act system is properly connected to the start of the game / end
        // of tutorial
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            BeginAct();
        }
    }

    #region Act Functions

    /// <summary>
    /// Initializes act variables
    /// </summary>
    private void InitializeActVariables()
    {
        _currentActNum = 0;
        _currentAct = _bossFightActs[_currentActNum];
    }

    /// <summary>
    /// Begins the next act
    /// </summary>
    private void BeginAct()
    {
        ResetSceneVariables();
        BeginScene();
        InvokeBeginActEvent();
    }

    /// <summary>
    /// Returns whether or not the act is complete
    /// </summary>
    private bool IsActCompleted()
    {
        return (_currentSceneNum == _currentAct.Scenes.Length);
    }

    /// <summary>
    /// Act end for attack
    /// </summary>
    private void OnActCompleted()
    {
        _currentActNum++;

        // Boss fight is over if all acts have been completed
        if(_currentActNum == _bossFightActs.Length)
        {
            //  TODO: End Game Here
            return;
        }

        _currentAct = _bossFightActs[_currentActNum];

        BeginAct();
        InvokeActEndEvent();
    }

    #endregion Act Functions

    #region ActScene Functions

    /// <summary>
    /// Initializes act variables
    /// </summary>
    private void InitializeSceneVariables()
    {
        _currentSceneNum = 0;
    }

    /// <summary>
    /// Returns whether or not the scene is complete
    /// </summary>
    private bool IsSceneCompleted()
    {
        return _attackCounter == _currentScene.SceneAttacks.Length;
    }

    /// <summary>
    /// Calls when a scene has been completed, progresses to the next scene or act
    /// </summary>
    private void OnSceneCompleted()
    {
        _currentSceneNum++;

        RemoveActAttackListeners();

        // Progresses to the next act if all scenes are completed
        if (IsActCompleted())
        {
            OnActCompleted();
            return;
        }

        _currentScene = _currentAct.Scenes[_currentSceneNum];
        // Begin next scene
        BeginScene();
        InvokeSceneEndEvent();
    }

    /// <summary>
    /// Sets the current scene num var back to 0
    /// </summary>
    private void ResetSceneVariables()
    {
        _currentSceneNum = 0;
        _currentScene = _currentAct.Scenes[_currentSceneNum];
    }

    /// <summary>
    /// Begins the next scene in the act
    /// </summary>
    private void BeginScene()
    {
        InitializeSceneAttackListeners();

        foreach (BaseBossAttack baseBossAttack in _currentScene.SceneAttacks)
        {
            baseBossAttack.InvokeAttackBegin();
        }
    }

    #endregion

    #region Attack Functions

    /// <summary>
    /// Adds listeners for all boss attacks
    /// </summary>
    private void InitializeSceneAttackListeners()
    {
        foreach (BaseBossAttack baseBossAttack in _currentScene.SceneAttacks)
        {
            baseBossAttack.GetAttackEndEvent().AddListener(AttackHasEnded);
        }
    }

    /// <summary>
    /// Removes all listeners from the act attacks
    /// </summary>
    private void RemoveActAttackListeners()
    {
        Act act = _bossFightActs[_currentActNum];

        foreach (BaseBossAttack baseBossAttack in _currentScene.SceneAttacks)
        {
            baseBossAttack.GetAttackEndEvent().RemoveListener(AttackHasEnded);
        }
    }

    /// <summary>
    /// Method for GetAttackEnd so the act system 
    /// </summary>
    private void AttackHasEnded()
    {
        _attackCounter++;

        if(IsSceneCompleted())
        {
            OnSceneCompleted();
        }
    }

    #endregion Attack Functions

    #region Events

    /// <summary>
    /// Act beginning for attack
    /// </summary>
    private void InvokeBeginActEvent()
    {
        _actBegin?.Invoke();
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeActEndEvent()
    {
        _actEnd?.Invoke();
    }

    /// <summary>
    /// Scene beginning for attack
    /// </summary>
    private void InvokeBeginSceneEvent()
    {
        _sceneBegin?.Invoke();
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeSceneEndEvent()
    {
        _sceneEnd?.Invoke();
    }

    #endregion

    #region Getters

    public UnityEvent GetActBegin() => _actBegin;
    public UnityEvent GetActEnd() => _actEnd;

    #endregion
}
