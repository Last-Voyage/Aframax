/******************************************************************************
// File Name:       SaveManager.cs
// Author:          Nick Rice
// Creation Date:   April 14, 2025
//
// Description:     Contains the functionality to set up and get access to Ui changes
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiManager : MainUniversalManagerFramework
{
    public static UiManager Instance;

    private static bool _usingController;
    private readonly UnityEvent _onSwapInput;

    /// <summary>
    /// This sends out the event to change the current Ui used in game when a controller is used
    /// For the time being the main usage of this is for changing a boolean
    /// </summary>
    public void SwapInput()
    {
        _usingController = !_usingController;
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

    /// <summary>
    /// Sets up the main manager by establishing the path to the Json file and loading the data
    /// </summary>
    public override void SetUpMainManager()
    {
        base.SetUpMainManager();
    }
    #endregion

    #region Getters

    public static bool UsingController => _usingController;

    public UnityEvent GetOnSwapInput => _onSwapInput;

    #endregion
}
