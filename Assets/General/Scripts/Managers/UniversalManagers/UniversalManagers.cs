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
    /// </summary>
    private MainUniversalManagerFramework[] _allMainManagers;
    public static UniversalManagers Instance;

    private ObjectPoolingParent _objectPoolingParent;
    private FmodSfxEvents _fModSfxEvents;
    private SceneTransitionBehaviour _transitionsBehaviour;

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
            // Parent needs to be removed at runtime in order for DontDestroyOnLoad to work
            transform.parent = null;
            //Don't destroy
            DontDestroyOnLoad(gameObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all managers that fall under universal managers
    /// </summary>
    protected override void GetAllManagers()
    {
        _allMainManagers = GetComponentsInChildren<MainUniversalManagerFramework>();
    }

    /// <summary>
    /// Tells all main gameplay managers to setup in the order of the main managers list
    /// </summary>
    protected override void SetupMainManagers()
    {
        SetupObjectPoolingParent();
        SetUpFModSfxEvents();
        SetUpSceneTransitions();

        //Instances all managers
        foreach (MainUniversalManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetUpInstance();
        }

        //Thens sets them up
        //They are instanced first so that if any manager needs to access any other manager in it's setup
        //  then the order doesn't matter
        foreach (MainUniversalManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetUpMainManager();
        }
        _objectPoolingParent.SubscribeToEvents();
    }

    /// <summary>
    /// Sets up the object pooling parent by establishing it's instance
    /// This is done here to make certain it happens before anything else
    /// </summary>
    private void SetupObjectPoolingParent()
    {
        _objectPoolingParent = GetComponentInChildren<ObjectPoolingParent>();
        _objectPoolingParent.SetupInstance();
    }

    private void SetUpFModSfxEvents()
    {
        _fModSfxEvents = GetComponentInChildren<FmodSfxEvents>();
        _fModSfxEvents.SetUpInstance();
    }

    private void SetUpSceneTransitions()
    {
        _transitionsBehaviour = GetComponentInChildren<SceneTransitionBehaviour>();
        _transitionsBehaviour.Setup();
    }

    #region Getters
    public MainUniversalManagerFramework[] GetAllUniversalManagers() => _allMainManagers;
    #endregion
}
