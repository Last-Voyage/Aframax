/******************************************************************************
// File Name:       CameraManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrew Stapay
// Creation Date:   September 15, 2024
//
// Description:     Provides functionality to how the camera moves and interacts.
******************************************************************************/

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
        TimeManager.Instance.GetOnGamePauseToggleEvent().AddListener(OnInvokeCameraMovementToggle);
    }

    /// <summary>
    /// Unsubscribes from all events on destruction
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        TimeManager.Instance.GetOnGamePauseToggleEvent().RemoveListener(OnInvokeCameraMovementToggle);
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
    
    #endregion
}
