/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson
// Contributor:     Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Events;

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
        else
        {
            Destroy(this);  
        }
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
        RuntimeSfxManager.Instance.InitializeFootstepInstance();
    }

    #region Getters
    public bool IsPlayerAboveDeck()
    {
        return _currentGameplayState != EGameplayState.BelowDeck;
    }

    public UnityEvent GetOnCompletedTutorialSection() => _onCompletedTutorialSection;

    public UnityEvent GetOnCompletedEntireTutorial() => _onCompletedEntireTutorial;
    public UnityEvent GetOnGamePaused() => _onGamePaused;
    public UnityEvent GetOnGameUnpaused() => _onGameUnpaused;
    
    public UnityEvent<ScriptableDialogueUi> GetOnNewDialogueChain() => _onNewDialogueChain;
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
