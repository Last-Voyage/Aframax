/******************************************************************************
// File Name:       EnvironmentManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides the functionality behind the way the environment interects with other elements
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality behind the way the environment interacts with other elements
/// Manager to be developed as I know specifics
/// </summary>
public class EnvironmentManager : MainGameplayManagerFramework
{
    public static EnvironmentManager Instance;
    #region Base Manager
    public override void SetupMainManager()
    {
        base.SetupMainManager();
        Instance = this;
    }
    #endregion

    #region Getters

    #endregion
}
