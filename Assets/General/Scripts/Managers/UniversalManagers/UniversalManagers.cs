using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instanced to allow for anything to use the manager
/// Provides access to all universal managers
/// </summary>
public class UniversalManagers : CoreManagersFramework
{
    public static UniversalManagers Instance;

    [SerializeField] private SceneLoadingManager _sceneLoadingManager;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TimeManager _timeManager;

    [Space]
    [SerializeField] private List<MainUniversalManagerFramework> _allMainManagers;

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
        foreach (MainUniversalManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetupMainManager();
        }
    }


    #region Getters
    public SceneLoadingManager GetSceneLoadingManager() => _sceneLoadingManager;
    public SaveManager GetSaveManager() => _saveManager;
    public AudioManager GetAudioManager() => _audioManager;
    public TimeManager GetTimeManager() => _timeManager;


    public List<MainUniversalManagerFramework> GetAllUniversalManagers() => _allMainManagers;
    #endregion
}
