/******************************************************************************
// File Name:       ControlsManager.cs
// Author:          Nick Rice
// Creation Date:   June 22, 2025
//
// Description:     Contains the functionality to set up and get access to controls changes
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

/// <summary>
/// 
/// </summary>
public class ControlsManager : MainUniversalManagerFramework
{
    public static ControlsManager Instance;
    private readonly UnityEvent<InputDevice> _onChangeControls = new();
    private InputDevice _currentDevice;
    private bool canUseControlSwap = true;
    private WaitForEndOfFrame _endOfFrame;

    private void ControlSwap(InputUser user, InputUserChange userChange, InputDevice device)
    {
        // Guard statement preventing:
        // If the last device it swapped to is the current device
        // If the device doesn't exist
        // If the end of the frame hasn't passed from the last control swap
        if (device == null || _currentDevice == device || !canUseControlSwap)
        {
            return;
        }
        
        _onChangeControls?.Invoke(device);
        _currentDevice = device;
        canUseControlSwap = false;
        StartCoroutine(PreventControlSwapUntilEndOfFrame());
    }


    /// <summary>
    /// This waits for the end of the frame before allowing the control swap to activate again
    /// </summary>
    private IEnumerator PreventControlSwapUntilEndOfFrame()
    {
        yield return _endOfFrame;
        canUseControlSwap = true;
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

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        InputUser.onChange += ControlSwap;
    }

    protected override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        InputUser.onChange -= ControlSwap;
    }
}