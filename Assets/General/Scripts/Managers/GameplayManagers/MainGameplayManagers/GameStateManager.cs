/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson
// Contributor:     Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

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
    
    private readonly UnityEvent<ScriptableDialogueUI> _onNewDialogueChain = new();

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
        }
        else
        {
            Destroy(this);  
        }
    }

    #endregion

    #region Getters

    public UnityEvent GetOnCompletedTutorialSection() => _onCompletedTutorialSection;

    public UnityEvent GetOnCompletedEntireTutorial() => _onCompletedEntireTutorial;
    public UnityEvent GetOnGamePaused() => _onGamePaused;
    public UnityEvent GetOnGameUnpaused() => _onGameUnpaused;
    
    public UnityEvent<ScriptableDialogueUI> GetOnNewDialogueChain() => _onNewDialogueChain;

    #endregion
}

/// <summary>
/// Various states of the game
/// </summary>
public enum EGameplayState
{
    TempState
}
