using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instanced to allow for anything to use the manager
/// Provides access to all gameplay managers
/// </summary>
public class GameplayManagers : CoreManagersFramework
{
    public static GameplayManagers Instance;

    [SerializeField] private GameStateManager _gameStateManager;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private EnvironmentManager _environmentManager;
    [SerializeField] private CameraManager _cameraManager;

    [Space]
    [SerializeField] private List<MainGameplayManagerFramework> _allMainManagers;

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
        foreach (MainGameplayManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetupMainManager();
        }
    }

    #region Getters
    public GameStateManager GetGameStateManager() => _gameStateManager;
    public PlayerManager GetPlayerManager() => _playerManager;
    public EnemyManager GetEnemyManager() => _enemyManager;
    public EnvironmentManager GetEnvironmentManager() => _environmentManager;
    public CameraManager GetCameraManager() => _cameraManager;
    #endregion
}
