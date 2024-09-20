/******************************************************************************
// File Name:       GameplayManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Instanced to allow for anything to use the manager
                    Provides access to all gameplay managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instanced to allow for anything to use the manager
/// Provides access to all gameplay managers
/// </summary>
public class GameplayManagers : CoreManagersFramework
{
    /// <summary>
    /// Contains all managers to setup. Order of managers is order of setup.
    /// Order shouldn't technically matter but just in case
    /// </summary>
    [SerializeField] private List<MainGameplayManagerFramework> _allMainGameplayManagers;

    public static GameplayManagers Instance;

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    /// <returns></returns>
    protected override bool EstablishInstance()
    {
        //If no other version exists
        if (Instance == null)
        {
            //This is the new singleton
            Instance = this;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tells all main gameplay managers to setup in the order of the main managers list
    /// </summary>
    protected override void SetupMainManagers()
    {
        foreach (MainGameplayManagerFramework mainManager in _allMainGameplayManagers)
        {
            mainManager.SetupMainManager();
        }
    }

    #region Getters

    #endregion
}
