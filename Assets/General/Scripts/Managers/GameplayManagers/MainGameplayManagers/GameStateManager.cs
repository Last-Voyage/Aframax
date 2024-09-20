/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds and moves through the states of gameplay
/// Manager to be developed as I know specifics
/// </summary>
public class GameStateManager : MainGameplayManagerFramework
{
    private EGameplayState _currentGameplayState;

    public static GameStateManager Instance;


    private void ChangeCurrentGameplayState(EGameplayState newState)
    {
        _currentGameplayState = newState;
    }

    #region Base Manager
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        Instance = this;
    }
    #endregion

    #region Getters

    #endregion
}

public enum EGameplayState
{
    tempState
};
