/******************************************************************************
// File Name:       UniversalManagers.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Instanced to allow for anything to use the manager
                    Provides access to all universal managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instanced to allow for anything to use the manager
/// Provides access to all universal managers
/// </summary>
public class UniversalManagers : CoreManagersFramework
{
    /// <summary>
    /// Contains all managers to setup. Order of managers is order of setup.
    /// Order shouldn't technically matter but just in case
    /// </summary>
    [SerializeField] private List<MainUniversalManagerFramework> _allMainManagers;

    public static UniversalManagers Instance;

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
            //Don't destroy
            DontDestroyOnLoad(gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Tells all main gameplay managers to setup in the order of the main managers list
    /// </summary>
    protected override void SetupMainManagers()
    {
        //Instances all managers
        foreach (MainUniversalManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetupInstance();
        }

        //Thens sets them up
        //They are instanced first so that if any manager needs to access any other manager in it's setup
        //  then the order doesn't matter
        foreach (MainUniversalManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetupMainManager();
        }
    }

    #region Getters
    public List<MainUniversalManagerFramework> GetAllUniversalManagers() => _allMainManagers;
    #endregion
}