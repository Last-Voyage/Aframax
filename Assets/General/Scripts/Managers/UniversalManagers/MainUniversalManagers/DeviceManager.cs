/******************************************************************************
// File Name:       DeviceManager.cs
// Author:          Nick Rice
// Creation Date:   March 10, 2025
//
// Description:     Manages the devices being used to play the game, and alerting
//                  scripts as this changes
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeviceManager : MainUniversalManagerFramework
{
    public static DeviceManager Instance;

    private static InputDevice _currentPlayerDevice;
    private static List<InputDevice> _allPlayerDevices;

    // Why is this being used instead of the built-in function?
    // Because I want one centralized place that uses it? But each script would need to use it anyways
    private static readonly UnityEvent<InputDevice> _onDeviceSwap;
    
    
    
    
    #region Base Manager
    /// <summary>
    /// Establishes the instance for the device manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    #endregion

    #region Getters

    public UnityEvent<InputDevice> GetOnDeviceSwap() => _onDeviceSwap;
    
    #endregion
    
    #region Events

    private void OnDeviceSwap(InputDevice device)
    {
        _onDeviceSwap?.Invoke(device);
    }

    #endregion

    private void UpdateDevices(InputDevice device,InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                //break;
            case InputDeviceChange.Reconnected:
                _allPlayerDevices.Add(device);
                OnDeviceSwap(device);
                break;
            case InputDeviceChange.Removed:
                // Removing it for both disconnected and removed will have to be changed later
            case InputDeviceChange.Disconnected:
                _allPlayerDevices.Remove(device);
                break;
        }
    }
    
    

    private void OnEnable()
    {
        _allPlayerDevices = InputSystem.devices.ToList();
        
        // Used to detect when devices are added or removed
        InputSystem.onDeviceChange += (device, change) => UpdateDevices(device, change);
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= (device, change) => UpdateDevices(device, change);
    }
}
