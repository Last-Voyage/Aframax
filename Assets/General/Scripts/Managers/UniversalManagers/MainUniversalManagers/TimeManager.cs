/******************************************************************************
// File Name:       TimeManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Controls the rate at which time moves
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality behind the speed at which time moves
/// Manager to be developed as I know specifics
/// </summary>
public class TimeManager : MainUniversalManagerFramework
{
    public static TimeManager Instance;

    private bool _isGamePaused;

    // Listens for whenever the game pauses
    private static UnityEvent _gamePausedEvent = new();
    private static UnityEvent _gameUnpausedEvent = new();

    private static UnityEvent<bool> _gamePauseToggleEvent = new();

    #region General Time Management
    /// <summary>
    /// Toggles if the game is paused or unpaused
    /// </summary>
    public void PauseGameToggle()
    {
        if(_isGamePaused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    /// <summary>
    /// Pauses the game and invokes needed events
    /// </summary>
    private void PauseGame()
    {
        Time.timeScale = 0;
        InvokeOnGamePause();
    }

    /// <summary>
    /// Unpauses the game and invokes needed events
    /// </summary>
    private void UnpauseGame()
    {
        Time.timeScale = 1;
        InvokeGameUnpaused();
    }
    #endregion

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

    #region Events
    private void InvokeGamePauseToggle(bool toggle)
    {
        _gamePauseToggleEvent?.Invoke(toggle);
    }
    private void InvokeOnGamePause()
    {
        _gamePausedEvent?.Invoke();
        InvokeGamePauseToggle(true);
    }
    private void InvokeGameUnpaused()
    {
        _gameUnpausedEvent?.Invoke();
        InvokeGamePauseToggle(false);
    }
    #endregion

    #region Getters
    public bool GetIsGamePaused() => _isGamePaused;

    public UnityEvent<bool> GetGamePauseToggleEvent() => _gamePauseToggleEvent;

    public UnityEvent GetGamePauseEvent() => _gamePausedEvent;
    public UnityEvent GetGameUnpauseEvent() => _gameUnpausedEvent;
    #endregion
}
