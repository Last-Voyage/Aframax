/*****************************************************************************
// File Name :         BossAttackActSystem.cs
// Author :            Mark Hanson
// Contributor:        Andrea Swihart-DeCoster, Nick Rice
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

    public bool HasActBegun { get; private set; }

    /// <summary>
    /// Scene should only begin once. This is set to true when the act begins.
    /// </summary>
    /// <param name="hasActBegun"></param>
    public void SetHasActBegun(bool hasActBegun)
    {
        HasActBegun = hasActBegun;
    }
}

/// <summary>
/// Defines a scene within an act
/// </summary>
[System.Serializable]
public struct ActScene
{
    [field: Tooltip("Attacks within the Act")]
    [field: SerializeField] public BaseBossAttack[] SceneAttacks { get; private set; }

    public bool HasSceneBegun {  get; private set; }

    /// <summary>
    /// Scene should only begin once. This is set to true when the scene begins.
    /// </summary>
    /// <param name="hasSceneBegun"></param>
    public void SetHasSceneBegun(bool hasSceneBegun)
    {
        HasSceneBegun = hasSceneBegun;
    }
}

/// <summary>
/// A class that contains multiple functions for the act system that are updated within a coroutine
/// </summary>
public class BossAttackActSystem : MonoBehaviour
{
    public static BossAttackActSystem Instance;

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
    private UnityEvent _onActBegin = new();

    [Tooltip("Invoked when an act ends")]
    private UnityEvent _onActEnd = new();

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
    private UnityEvent _onSceneBegin = new();

    [Tooltip("Invoked when a scene ends")]
    private UnityEvent _onSceneEnd = new();

    [Tooltip("Invoked when an attack ends")]
    private UnityEvent _onAttackCompleted = new();
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        InitializeActVariables();
        InitializeSceneVariables();
    }

    /// <summary>
    /// Adds a listener to this script when enabled
    /// Allowing the new act to begin
    /// </summary>
    private void OnEnable()
    {
        //GetOnActBegin().AddListener(BeginAct);
    }

    /// <summary>
    /// Removes the listener - PREVENTING MEMORY LEAKS
    /// </summary>
    private void OnDisable()
    {
        //GetOnActBegin().RemoveListener(BeginAct);
    }

#if UNITY_EDITOR

    /// <summary>
    /// just for testing
    /// </summary>
    private void Update()
    {
        // Test the begin interior attack until act system is properly connected to the start of the game / end
        // of tutorial
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !TimeManager.Instance.GetIsGamePaused())
        {
            BeginAct();
        }
    }
    
#endif

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
        /// Scene should only begin once
        if (_currentAct.HasActBegun)
        {
            return;
        }

        ResetSceneVariables();
        BeginScene();
        InvokeBeginActEvent();

        _currentAct.SetHasActBegun(true);
        _currentActNum++;
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
    private void CompleteAct()
    {
        // Boss fight is over if all acts have been completed
        if(_currentActNum == _bossFightActs.Length)
        {
            SceneLoadingManager.Instance.InvokeEndOfGameScene();
            SceneLoadingManager.Instance.StartAsyncSceneLoadViaID(3, 0);
            Debug.Log("Act ended");
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
    /// Begins the next scene in the act
    /// </summary>
    private void BeginScene()
    {
        /// Scene should only begin once
        if(_currentScene.HasSceneBegun)
        {
            return;
        }

        InitializeSceneAttackListeners();

        foreach (BaseBossAttack baseBossAttack in _currentScene.SceneAttacks)
        {
            baseBossAttack.InvokeAttackBegin();
        }
        InvokeBeginSceneEvent();
        _currentScene.SetHasSceneBegun(true);
        _currentSceneNum++;
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
    private void CompleteScene()
    {
        RemoveActAttackListeners();

        // Progresses to the next act if all scenes are completed
        if (IsActCompleted())
        {
            CompleteAct();
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
        _onAttackCompleted?.Invoke();

        _attackCounter++;

        if(IsSceneCompleted())
        {
            CompleteScene();
        }
    }

    #endregion Attack Functions

    #region Events

    /// <summary>
    /// Act beginning for attack
    /// </summary>
    private void InvokeBeginActEvent()
    {
        _onActBegin?.Invoke();
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeActEndEvent()
    {
        _onActEnd?.Invoke();
    }

    /// <summary>
    /// Scene beginning for attack
    /// </summary>
    private void InvokeBeginSceneEvent()
    {
        _onSceneBegin?.Invoke();
        RuntimeSfxManager.APlayOneShotSFX?.Invoke
            (FmodSfxEvents.Instance.SceneStart, PlayerMovementController.Instance.transform.position);
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void InvokeSceneEndEvent()
    {
        _onSceneEnd?.Invoke();
    }
    
    #endregion

    #region Getters

    public UnityEvent GetOnActBegin() => _onActBegin;
    public UnityEvent GetOnActEnd() => _onActEnd;
    public UnityEvent GetOnSceneBegin() => _onSceneBegin;
    public UnityEvent GetOnSceneEnd() => _onSceneEnd;

    public UnityEvent GetOnAttackCompleted() => _onAttackCompleted;

    #endregion
}
