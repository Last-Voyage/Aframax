/******************************************************************************
// File Name:       CameraManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Provides functionality to how the camera moves and interacts.
******************************************************************************/

using System.Linq.Expressions;
using UnityEngine.Events;

/// <summary>
/// Provides functionality to how the camera moves and interacts
/// Manager to be developed as I know specifics
/// </summary>
public class CameraManager : MainGameplayManagerFramework
{
    public static CameraManager Instance;

    /// <summary>
    /// Controls the player camera
    /// </summary>
    private static readonly UnityEvent<bool> _onCameraMovementToggled = new();

    /// <summary>
    /// Moves the camera during jumpscares
    /// </summary>
    private static readonly UnityEvent _onJumpscare = new();

    /// <summary>
    /// Manages the camera for cinematic events during gameplay
    /// </summary>
    private static readonly UnityEvent _onCinematicStart = new();
    private static readonly UnityEvent _onCinematicEnd = new();

    /// <summary>
    /// Invokes the _onCameraMovementToggled event when the game is paused
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    /// <param name="audioToggle"> the bool to toggle audio on pause</param>
    private void CameraMovementOnPause(bool toggle, bool audioToggle)
    {
        OnInvokeCameraMovementToggle(toggle);
    }

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the camera manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    /// <summary>
    /// Subscribes to all required events
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        //Disables camera movement on game pause
        TimeManager.Instance.GetOnGamePauseToggleEvent().AddListener(CameraMovementOnPause);
    }

    /// <summary>
    /// Unsubscribes from all events on destruction
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        TimeManager.Instance.GetOnGamePauseToggleEvent().RemoveListener(CameraMovementOnPause);
    }
    
    #endregion

    #region Events
    
    /// <summary>
    /// Invokes the _onCameraMovementToggled event with the input bool
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    public void OnInvokeCameraMovementToggle(bool toggle)
    {
        _onCameraMovementToggled?.Invoke(!toggle);
    }

    /// <summary>
    /// Invokes the _onJumpscare event
    /// </summary>
    public void InvokeOnJumpscare()
    {
        _onJumpscare?.Invoke();
    }

    /// <summary>
    /// Invokes the _onCinematicStart event
    /// </summary>
    public void InvokeOnCinematicStart()
    {
        _onCinematicStart?.Invoke();
    }

    /// <summary>
    /// Invokes the _onCinematicEnd event
    /// </summary>
    public void InvokeOnCinematicEnd()
    {
        _onCinematicEnd?.Invoke();
    }
    
    #endregion

    #region Getters
    
    /// <summary>
    /// Getter for the _onCameraMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetOnCameraMovementToggleEvent() => _onCameraMovementToggled;

    /// <summary>
    /// Getter for the _onJumpscare event
    /// </summary>
    public UnityEvent GetOnJumpscareEvent() => _onJumpscare;

    /// <summary>
    /// Getter for the _onCinematicStart event
    /// </summary>
    public UnityEvent GetOnCinematicStartEvent() => _onCinematicStart;

    /// <summary>
    /// Getter for the _onCinematicEnd event
    /// </summary>
    /// <returns></returns>
    public UnityEvent GetOnCinematicEndEvent() => _onCinematicEnd;
    
    #endregion
}
