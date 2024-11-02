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

    private readonly UnityEvent _onCompletedTutorial = new();

    /// <summary>
    /// Switches gameplay state 
    /// </summary>
    /// <param name="newState"> new gameplay state </param>
    private void ChangeCurrentGameplayState(EGameplayState newState)
    {
        _currentGameplayState = newState;
    }

    #region Base Manager
    
    public override void SetupInstance()
    {
        base.SetupInstance();
        Instance = this;
    }
    
    #endregion

    #region Getters

    public UnityEvent GetOnCompletedTutorial() => _onCompletedTutorial;

    #endregion
}

/// <summary>
/// Various states of the game
/// </summary>
public enum EGameplayState
{
    TempState
}
