/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson
// Contributors:    Nick Rice, Adam Garwacki
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Holds and moves through the states of gameplay
/// Manager to be developed as I know specifics
/// </summary>
public class GameStateManager : MainGameplayManagerFramework
{
    private EGameplayState _currentGameplayState;

    public static GameStateManager Instance;

    private readonly UnityEvent _onCompletedTutorialSection = new();

    private readonly UnityEvent _onCompletedEntireTutorial = new();
    
    private readonly UnityEvent _onGamePaused = new();
    private readonly UnityEvent _onGameUnpaused = new();

    private readonly UnityEvent<ScriptableDialogueUi> _onNewDialogueChain = new();
    private readonly UnityEvent<ScriptableDialogueUi> _onDialogueProgress = new();

    [SerializeField] private float _loadBufferTime;
    private bool _isGameLoading;

    /// <summary>
    /// Switches gameplay state 
    /// </summary>
    /// <param name="newState"> new gameplay state </param>
    private void ChangeCurrentGameplayState(EGameplayState newState)
    {
        _currentGameplayState = newState;
    }

    #region Base Manager

    private void Awake()
    {
        SetUpInstance();
        StartCoroutine(LoadingBuffer());
    }
    
    /// <summary>
    /// Establishes the instance for the game state manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        
        if(Instance == null)
        {
            Instance = this;
            GetLocationState();
        }
    }

    /// <summary>
    /// Freezes player inputs to give assets a chance to load upon level load
    /// </summary>
    /// <returns>Time allowed for loading</returns>
    private IEnumerator LoadingBuffer()
    {
        // Pauses the game temporarily to let objects load
        PauseMenu.Instance.PauseToggle();
        _isGameLoading = true;
        PauseMenu.Instance.ToggleLightScrim(true);
        yield return new WaitForSecondsRealtime(_loadBufferTime);

        // Unpauses the game and lets player see game world
        _isGameLoading = false;
        PauseMenu.Instance.ToggleLightScrim(false);
        PauseMenu.Instance.PauseToggle();
    }
    #endregion

    /// <summary>
    /// Determines if you are above or below deck
    /// Changes the state and updates the footstep audio
    /// </summary>
    private void GetLocationState()
    {
        if (AframaxSceneManager.Instance.IsAboveDeck())
        {
            _currentGameplayState = EGameplayState.AboveDeck;
        }
        else if (AframaxSceneManager.Instance.IsBelowDeck())
        {
            _currentGameplayState = EGameplayState.BelowDeck;
        }
        RuntimeSfxManager.Instance.InitializeFootstepInstances();
    }

    #region Getters
    public bool IsPlayerAboveDeck()
    {
        return _currentGameplayState != EGameplayState.BelowDeck;
    }

    /// <summary>
    /// Whether the game is loading. Loading occurs for 3 seconds when maze scene is awoken.
    /// </summary>
    /// <returns>The game's loading state.</returns>
    public bool IsGameLoading()
    {
        return _isGameLoading;
    }

    public UnityEvent GetOnCompletedTutorialSection() => _onCompletedTutorialSection;

    public UnityEvent GetOnCompletedEntireTutorial() => _onCompletedEntireTutorial;
    public UnityEvent GetOnGamePaused() => _onGamePaused;
    public UnityEvent GetOnGameUnpaused() => _onGameUnpaused;

    public UnityEvent<ScriptableDialogueUi> GetOnNewDialogueChain() => _onNewDialogueChain;
    public UnityEvent<ScriptableDialogueUi> GetOnDialogueProgress() => _onDialogueProgress;

    #endregion
}

/// <summary>
/// Various states of the game
/// </summary>
public enum EGameplayState
{
    AboveDeck,
    BelowDeck,
    Ending
};