using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManagers : CoreManagersFramework
{
    public static GameplayManagers Instance;

    [SerializeField] private GameStateManager _gameStateManager;

    [SerializeField] private List<MainGameplayManagerFramework> _allMainManagers;

    protected override bool EstablishInstance()
    {
        Instance = this;
        return true;
    }

    protected override void SetupMainManagers()
    {
        foreach (MainGameplayManagerFramework mainManager in _allMainManagers)
        {
            mainManager.SetupMainManager();
        }
    }

    #region Getters
    public GameStateManager GetGameStateManager() => _gameStateManager;
    #endregion
}
