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

    /// <summary>
    /// First bool is if the game is toggled. Second bool is for if the audio should pause
    /// </summary>
    private static readonly UnityEvent<bool,bool> _onGamePauseToggleEvent = new();

    #region General Time Management
    /// <summary>
    /// Toggles if the game is paused or unpaused
    /// </summary>
    /// <param name="doesToggleAudio"> Toggle to pause audio</param>
    public void PauseGameToggle(bool doesToggleAudio)
    {
        _isGamePaused = !_isGamePaused;
        if(_isGamePaused)
        {
            PauseGame(doesToggleAudio);
        }
        else
        {
            UnpauseGame(doesToggleAudio);
        }
    }

    /// <summary>
    /// Pauses the game and invokes needed events
    /// </summary>
    /// <param name="doesToggleAudio"> Toggle to pause audio</param>
    private void PauseGame(bool doesToggleAudio)
    {
        Time.timeScale = 0;
        OnInvokeGamePause(doesToggleAudio);
    }

    /// <summary>
    /// Unpauses the game and invokes needed events
    /// </summary>
    /// <param name="doesToggleAudio"> Toggle to pause audio</param>
    private void UnpauseGame(bool doesToggleAudio)
    {
        Time.timeScale = 1;
        OnInvokeGameUnpaused(doesToggleAudio);
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
    /// <param name="doesToggleAudio"> Toggle to pause audio</param>
    private void OnInvokeGamePauseToggle(bool isPaused, bool doesToggleAudio)
    {
        _onGamePauseToggleEvent?.Invoke(isPaused, doesToggleAudio);
    }
    
    /// <summary>
    /// Invokes the game pause
    /// </summary>
    /// <param name="doesToggleAudio"></param>
    private void OnInvokeGamePause(bool doesToggleAudio)
    {
        _onGamePausedEvent?.Invoke();
        OnInvokeGamePauseToggle(true,doesToggleAudio);
    }
    
    /// <summary>
    /// Invokes the game unpause
    /// </summary>
    /// <param name="doesToggleAudio"></param>
    private void OnInvokeGameUnpaused(bool doesToggleAudio)
    {
        _onGameUnpausedEvent?.Invoke();
        OnInvokeGamePauseToggle(false,doesToggleAudio);
    }
    
    #endregion

    #region Getters
    public bool GetIsGamePaused() => _isGamePaused;

    public UnityEvent<bool,bool> GetOnGamePauseToggleEvent() => _onGamePauseToggleEvent;

    public UnityEvent GetOnGamePauseEvent() => _onGamePausedEvent;
    public UnityEvent GetOnGameUnpauseEvent() => _onGameUnpausedEvent;
    #endregion
}
