/******************************************************************************
// File Name:       ControlsManager.cs
// Author:          Nick Rice
// Creation Date:   June 22, 2025
//
// Description:     Contains the functionality to set up and get access to controls changes
******************************************************************************/

using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

/// <summary>
/// Contains the functionality to set up and get access to controls changes
/// </summary>
public class ControlsManager : MainUniversalManagerFramework
{
    public static ControlsManager Instance;
    private readonly UnityEvent<InputDevice> _onChangeControls = new();
    private InputDevice _currentDevice;
    private bool _canUseControlSwap = true;
    private WaitForEndOfFrame _endOfFrame;

    private void Start()
    {
        if (!Gamepad.current.IsUnityNull())
        {
            // We have 0 input users at the start...
            Debug.Log(InputUser.all.Count);
            ControlSwap(InputUser.all[0], InputUserChange.Added, Gamepad.current);
        }
        else
        {
            Debug.Log("Whos asking??");
            ControlSwap(InputUser.all[0], InputUserChange.Added, Keyboard.current);
        }
    }

    /// <summary>
    /// This sends an event when the player swaps controls
    /// </summary>
    /// <param name="user">The player that changed their controls</param>
    /// <param name="userChange">The change that happened with the user</param>
    /// <param name="device">The device that was changed to</param>
    private void ControlSwap(InputUser user, InputUserChange userChange, InputDevice device)
    {
        //Debug.Log(device);
        
        // Guard statement preventing:
        // If the last device it swapped to is the current device
        // If the device doesn't exist
        // If the end of the frame hasn't passed from the last control swap
        if (device == null || _currentDevice == device || !_canUseControlSwap ||
            userChange == InputUserChange.DeviceUnpaired)
        {
            return;
        }
        Debug.Log(device + " " + userChange);
        
        _onChangeControls?.Invoke(device);
        _currentDevice = device;
        _canUseControlSwap = false;
        StartCoroutine(PreventControlSwapUntilEndOfFrame());
    }


    /// <summary>
    /// This waits for the end of the frame before allowing the control swap to activate again
    /// </summary>
    private IEnumerator PreventControlSwapUntilEndOfFrame()
    {
        yield return _endOfFrame;
        _canUseControlSwap = true;
    }
    
    #region BaseManager
    /// <summary>
    /// Establishes the instance for the save manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }
    #endregion

    #region Getters

    public UnityEvent<InputDevice> GetOnChangeControls => _onChangeControls;

    #endregion

    /// <summary>
    /// Subscribes a general device change event to a filtered one that will be sent out to our scripts
    /// </summary>
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        InputUser.onChange += ControlSwap;
    }

    /// <summary>
    /// Removes the event to prevent memory leaks
    /// </summary>
    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        InputUser.onChange -= ControlSwap;
    }
}