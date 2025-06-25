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

public class ControlsManager : MainUniversalManagerFramework
{
    public static ControlsManager Instance;
    private readonly UnityEvent<InputDevice> _onChangeControls = new();
    private InputDevice _currentDevice;
    private bool canUseControlSwap = true;
    private WaitForEndOfFrame _endOfFrame;
    
    // Start is called before the first frame update
    void Start()
    {
        InputUser.onChange += ControlSwap;
    }

    private void ControlSwap(InputUser user, InputUserChange userChange, InputDevice device)
    {
        if (device == null || _currentDevice == device || !canUseControlSwap)
        {
            return;
        }
        
        _onChangeControls?.Invoke(device);
        _currentDevice = device;
        canUseControlSwap = false;
        StartCoroutine(WaitForThing());
    }


    /// <summary>
    /// This waits for the end of the frame before allowing the control swap to activate again
    /// </summary>
    private IEnumerator WaitForThing()
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
}

internal class PlayerInputDevice : InputDevice
{
    public static PlayerInputDevice CurrentPlayerInputDevice { get; internal set; }
    
    
}
