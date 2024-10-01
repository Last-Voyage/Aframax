/******************************************************************************
// File Name:       CameraManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides functionality to how the camera moves and interacts.
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private static UnityEvent<bool> _onCameraMovementToggled;
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
    /// <summary>
    /// Invokes the _onCameraMovementToggled event with the input bool
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    public void InvokeOnCameraMovementToggle(bool toggle)
    {
        _onCameraMovementToggled?.Invoke(toggle);
    }
    #endregion

    #region Getters
    /// <summary>
    /// Getter for the _onCameraMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetCameraMovementToggleEvent() => _onCameraMovementToggled;
    #endregion
}
