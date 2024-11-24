/******************************************************************************
// File Name:       GameplayManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 14, 2024
//
// Description:     Instanced to allow for anything to use the manager
                    Provides access to all gameplay managers
******************************************************************************/

/// <summary>
/// Instanced to allow for anything to use the manager
/// Provides access to all gameplay managers
/// </summary>
public class GameplayManagers : CoreManagersFramework
{
    /// <summary>
    /// Contains all managers to setup. Order of managers is order of setup.
    /// </summary>
    private MainGameplayManagerFramework[] _allMainGameplayManagers;

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
    /// Gets all managers that fall under gameplay manager
    /// </summary>
    protected override void GetAllManagers()
    {
        _allMainGameplayManagers = GetComponentsInChildren<MainGameplayManagerFramework>();
    }

    /// <summary>
    /// Tells all main gameplay managers to setup in the order of the main managers list
    /// </summary>
    protected override void SetupMainManagers()
    {
        //Instances all managers
        foreach (MainGameplayManagerFramework mainManager in _allMainGameplayManagers)
        {
            mainManager.SetUpInstance();
        }

        //Then sets them up
        //The managers perform their set up after establishing the instance so that they can access all other managers
        //For example subscribing to events from the other manager
        foreach (MainGameplayManagerFramework mainManager in _allMainGameplayManagers)
        {
            mainManager.SetUpMainManager();
        }

        //Informs the scene manager that a gameplay scene was loaded
        AframaxSceneManager.Instance.InvokeOnGameplaySceneLoaded();
    }
}
