/******************************************************************************
// File Name:       AudioManager.cs
// Author:          Ryan Swanson
// Contributors:    Andrea Swihart-DeCoster
// Creation Date:   September 14, 2024
//
// Description:     Provides the functionality behind all audio
                    Manager to be developed as I know specifics
******************************************************************************/
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

/// <summary>
/// Provides the functionality behind all audio
/// Manager to be developed
/// </summary>
public class AudioManager : MainUniversalManagerFramework
{
    public static AudioManager Instance;

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
