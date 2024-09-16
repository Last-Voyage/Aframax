using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds and moves through the states of gameplay
/// Manager to be developed as I know specifics
/// </summary>
public class GameStateManager : MainGameplayManagerFramework
{
    private GameplayState _currentGameplayState;


    private void ChangeCurrentGameplayState(GameplayState newState)
    {
        _currentGameplayState = newState;
    }

    #region Base Manager
    public override void SetupMainManager()
    {
        base.SetupMainManager();
    }
    #endregion

    #region Getters

    #endregion
}

public enum GameplayState
{
    tempState
};
