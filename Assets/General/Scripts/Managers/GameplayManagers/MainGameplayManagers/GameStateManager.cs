/******************************************************************************
// File Name:       GameStateManager.cs
// Author:          Ryan Swanson, Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Holds and moves through the states of gameplay
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds and moves through the states of gameplay
/// Manager to be developed as I know specifics
/// </summary>
public class GameStateManager : MainGameplayManagerFramework
{
    private EGameplayState _currentGameplayState;

    public static GameStateManager Instance;

    private UnityEvent _completedTutorial = new();


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
    public override void SetupMainManager()
    {
        base.SetupMainManager();
    }
    #endregion

    #region Getters

    public UnityEvent CompletedTutorial() => _completedTutorial;

    #endregion
}

public enum EGameplayState
{
    tempState
};
