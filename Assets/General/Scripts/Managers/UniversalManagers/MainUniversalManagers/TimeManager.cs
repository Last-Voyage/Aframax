/******************************************************************************
// File Name:       TimeManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Controls the rate at which time moves
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality behind the speed at which time moves
/// Manager to be developed as I know specifics
/// </summary>
public class TimeManager : MainUniversalManagerFramework
{
    public static TimeManager Instance;
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

    #region Getters

    #endregion
}