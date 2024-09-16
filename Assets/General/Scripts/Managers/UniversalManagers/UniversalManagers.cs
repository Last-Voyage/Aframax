using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalManagers : CoreManagersFramework
{
    public static UniversalManagers Instance;

    [SerializeField] private SceneLoadingManager _sceneLoadingManager;
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TimeManager _timeManager;

    [Space]
    [SerializeField] private List<MainUniversalManagerFramework> _allMainManagers;

    protected override bool EstablishInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            return true;
        }
        return false;
    }

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
