using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
