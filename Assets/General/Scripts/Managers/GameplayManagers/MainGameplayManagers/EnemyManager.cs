/******************************************************************************
// File Name:       EnemyManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides other scripts with access to the boss.
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides other scripts with access to the boss
/// Manager to be developed as I know specifics
/// </summary>
public class EnemyManager : MainGameplayManagerFramework
{
    public static EnemyManager Instance;
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
