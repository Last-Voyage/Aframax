/******************************************************************************
// File Name:       TimeManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Controls the rate at which time moves
                    Manager to be developed as I know specifics
******************************************************************************/

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
    private static readonly UnityEvent _onGamePausedEvent = new();
    private static readonly UnityEvent _onGameUnpausedEvent = new();

    private static readonly UnityEvent<bool> _onGamePauseToggleEvent = new();

    #region General Time Management
    /// <summary>
    /// Toggles if the game is paused or unpaused
    /// </summary>
    public void PauseGameToggle()
    {
        _isGamePaused = !_isGamePaused;
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
        InvokeOnGameUnpaused();
    }
    #endregion

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the time manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    #endregion

    #region Events

    /// <summary>
    /// Toggles the game being paused or unpaused
    /// </summary>
    /// <param name="isPaused"> Toggle for if the game is paused or not </param>
    private void InvokeOnGamePauseToggle(bool isPaused)
    {
        _onGamePauseToggleEvent?.Invoke(isPaused);
    }
    
    private void InvokeOnGamePause()
    {
        _onGamePausedEvent?.Invoke();
        InvokeOnGamePauseToggle(true);
    }
    
    private void InvokeOnGameUnpaused()
    {
        _onGameUnpausedEvent?.Invoke();
        InvokeOnGamePauseToggle(false);
    }
    
    #endregion

    #region Getters
    public bool GetIsGamePaused() => _isGamePaused;

    public UnityEvent<bool> GetOnGamePauseToggleEvent() => _onGamePauseToggleEvent;

    public UnityEvent GetOnGamePauseEvent() => _onGamePausedEvent;
    public UnityEvent GetOnGameUnpauseEvent() => _onGameUnpausedEvent;
    #endregion
}
