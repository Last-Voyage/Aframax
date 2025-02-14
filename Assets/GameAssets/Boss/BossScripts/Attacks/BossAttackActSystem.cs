/*****************************************************************************
// File Name :         BossAttackActSystem.cs
// Author :            Mark Hanson
// Contributor:        Andrea Swihart-DeCoster, Nick Rice, Ryan Swanson
// Creation Date :     10/22/2024
//
// Brief Description : The system to manage what act the boss is on and also
//                     switch between them along with which attack comes out
*****************************************************************************/

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
    [field: SerializeField]
    public ActScene[] Scenes { get; private set; }

    public bool HasActBegun { get; private set; }

    /// <summary>
    /// Scene should only begin once. This is set to true when the act begins.
    /// </summary>
    /// <param name="hasActBegun"> Boolean for setting if the act has begun </param>
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
    [field: SerializeField]
    public BaseBossAttack[] SceneAttacks { get; private set; }

    public bool HasSceneBegun {  get; private set; }

    /// <summary>
    /// Scene should only begin once. This is set to true when the scene begins.
    /// </summary>
    /// <param name="hasSceneBegun"> Boolean for setting if the scene has begun </param>
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

    [Tooltip("Acts within the boss fight")] [SerializeField]
    private Act[] _bossFightActs;

    /// <summary>
    /// Current act position in array
    /// </summary>
    private int _currentActNum;

    /// <summary>
    /// Current act ref
    /// </summary>
    private Act _currentAct;

    [Tooltip("Invoked when an act begins")]
    private readonly UnityEvent _onActBegin = new();

    [Tooltip("Invoked when an act ends")] private readonly UnityEvent _onActEnd = new();

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

    [Tooltip("# of attacks completed")] private int _attackCounter;

    [Tooltip("Invoked when a scene begins")]
    private readonly UnityEvent _onSceneBegin = new();

    [Tooltip("Invoked when a scene ends")] private readonly UnityEvent _onSceneEnd = new();

    [Tooltip("Invoked when an attack ends")]
    private readonly UnityEvent _onAttackCompleted = new();

    #endregion

    private void Awake()
    {
        if (Instance == null)
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
        GameStateManager.Instance.GetOnCompletedEntireTutorial().AddListener(BeginAct);
    }

    /// <summary>
    /// Removes the listener - PREVENTING MEMORY LEAKS
    /// </summary>
    private void OnDisable()
    {
        GameStateManager.Instance.GetOnCompletedEntireTutorial().RemoveListener(BeginAct);
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
        GameStateManager.Instance.GetOnCompletedEntireTutorial().RemoveListener(BeginAct);
        // Scene should only begin once
        if (_currentAct.HasActBegun)
        {
            return;
        }

        ResetSceneVariables();
        BeginScene();
        OnInvokeBeginActEvent();

        _currentAct.SetHasActBegun(true);
        _currentActNum++;
    }

    /// <summary>
    /// Whether the act is complete
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
        if (_currentActNum == _bossFightActs.Length)
        {
            AframaxSceneManager.Instance.LoadEndScene();
            return;
        }

        _currentAct = _bossFightActs[_currentActNum];

        BeginAct();
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(3, 0);
        OnInvokeActEndEvent();
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
        // Scene should only begin once
        if(_currentScene.HasSceneBegun)
        {
            return;
        }

        InitializeSceneAttackListeners();

        foreach (BaseBossAttack baseBossAttack in _currentScene.SceneAttacks)
        {
            baseBossAttack.InvokeAttackBegin();
        }
        OnInvokeBeginSceneEvent();
        _currentScene.SetHasSceneBegun(true);
        _currentSceneNum++;
    }

    /// <summary>
    /// Whether the scene is complete
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
        OnInvokeSceneEndEvent();
    }

    /// <summary>
    /// Stops all active attacks if needed
    /// Used by StoryManager to stop attacks
    /// </summary>
    public void ForceEndAllAttacks()
    {
        // I bet you are asking why are not just going through all attacks in the "current combat scenes"
        // Based on how the story manager is set up I'm assuming that "combat scenes are older functionality being 
        // removed so this is the alternative. Can't get the attacks in the current combat scene if there is no
        // combat scene
        // TODO rework the "Combat Scenes" functionality
        BaseBossAttack[] allAttacks = GetComponentsInChildren<BaseBossAttack>();
        foreach(BaseBossAttack attack in allAttacks)
        {
            attack.EndAttack();
        }

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

        if (IsSceneCompleted())
        {
            CompleteScene();
        }
    }

    #endregion Attack Functions

    #region Events

    /// <summary>
    /// Act beginning for attack
    /// </summary>
    private void OnInvokeBeginActEvent()
    {
        _onActBegin?.Invoke();
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void OnInvokeActEndEvent()
    {
        _onActEnd?.Invoke();
    }

    /// <summary>
    /// Scene beginning for attack
    /// </summary>
    private void OnInvokeBeginSceneEvent()
    {
        _onSceneBegin?.Invoke();
        RuntimeSfxManager.APlayOneShotSfx?.Invoke
            (FmodSfxEvents.Instance.SceneStart, PlayerMovementController.Instance.transform.position);
    }

    /// <summary>
    /// A way for other scripts to see the act ending
    /// If needed something can listen to those from another script to do something
    /// </summary>
    private void OnInvokeSceneEndEvent()
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
