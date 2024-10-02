/******************************************************************************
// File Name:       PlayerManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides other all other scripts with access to the player
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides all other scripts with access to the player
/// </summary>
public class PlayerManager : MainGameplayManagerFramework
{
    public static PlayerManager Instance;

    /// <summary>
    /// Controls the player's movement
    /// </summary>
    private static UnityEvent<bool> _onMovementToggled = new();

    #region Base Manager
    public override void SetupInstance()
    {
        base.SetupInstance();
    }
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        Instance = this;
    }
    #endregion

    #region Events
    /// <summary>
    /// Invokes the _onMovementToggled event with the input bool
    /// </summary>
    /// <param name="toggle"> the bool to input into the invoked event </param>
    public void InvokeOnCameraMovementToggle(bool toggle)
    {
        _onMovementToggled?.Invoke(toggle);
    }

    /// <summary>
    /// Invokes the harpoon fired event
    /// </summary>
    public void InvokeHarpoonFiredEvent()
    {
        _harpoonFiredEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the harpoon retract event
    /// </summary>
    public void InvokeHarpoonRetractEvent()
    {
        _harpoonRetractEvent?.Invoke();
    }

    /// <summary>
    /// Invokes the harpoon focus start event
    /// </summary>
    public void InvokeHarpoonFocusStartEvent()
    {
        _harpoonFocusStartEvent?.Invoke();
    }
    /// <summary>
    /// Invokes the harpoon focus max event
    /// </summary>
    public void InvokeHarpoonFocusMaxEvent()
    {
        _harpoonFocusMaxEvent?.Invoke();
    }
    /// <summary>
    /// Invokes the harpoon focus end event
    /// </summary>
    public void InvokeHarpoonEndEvent()
    {
        _harpoonFocusEndEvent?.Invoke();
    }
    #endregion

    #region Getters
    /// <summary>
    /// Getter for the _onMovementToggled event
    /// </summary>
    public UnityEvent<bool> GetMovementToggleEvent() => _onMovementToggled;
    #endregion
}
