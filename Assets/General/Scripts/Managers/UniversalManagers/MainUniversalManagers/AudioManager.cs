/******************************************************************************
// File Name:       AudioManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Provides the functionality behind all audio
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality behind all audio
/// Manager to be developed as I know specifics
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    public static AudioManager Instance;
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
