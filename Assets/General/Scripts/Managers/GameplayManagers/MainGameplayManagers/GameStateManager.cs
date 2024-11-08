/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson
// Contributor:     Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

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

    /// <summary>
    /// Switches gameplay state 
    /// </summary>
    /// <param name="newState"> new gameplay state </param>
    private void ChangeCurrentGameplayState(EGameplayState newState)
    {
        _currentGameplayState = newState;
    }

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the game state manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    #endregion

    #region Getters

    public UnityEvent GetOnCompletedTutorialSection() => _onCompletedTutorialSection;

    public UnityEvent GetOnCompletedEntireTutorial() => _onCompletedEntireTutorial;

    #endregion
}

/// <summary>
/// Various states of the game
/// </summary>
public enum EGameplayState
{
    TempState
}
