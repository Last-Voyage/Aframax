/******************************************************************************
// File Name:       SaveManager.cs
// Author:          Nick Rice
// Creation Date:   April 14, 2025
//
// Description:     Contains the functionality to set up and get access to Ui changes
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains the functionality to set up and get access to Ui changes
/// </summary>
public class UiManager : MainUniversalManagerFramework
{
    public static UiManager Instance;

    private static bool _isUsingController;
    private readonly UnityEvent _onSwapInput = new();

    /// <summary>
    /// Makes the controller toggle save between game sessions
    /// </summary>
    private void Awake()
    {
        _isUsingController = SaveManager.Instance.GetGameSaveData().IsUsingController;
    }

    /// <summary>
    /// This sends out the event to change the current Ui used in game when a controller is used
    /// For the time being the main usage of this is for changing a boolean
    /// </summary>
    public void SwapInput()
    {
        _isUsingController = !_isUsingController;
        SaveManager.Instance.GetGameSaveData().IsUsingController = _isUsingController;
        _onSwapInput?.Invoke();
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

    public static bool IsUsingController => _isUsingController;

    public UnityEvent GetOnSwapInput => _onSwapInput;

    #endregion
}
