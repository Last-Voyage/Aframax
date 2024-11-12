/*****************************************************************************
// File Name :         SettingsSceneBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     11/10/24
//
// Brief Description : handles using escape to leave the settings menu
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this makes it so u can press escape to return to the previous scene
/// </summary>
public class SettingsSceneBehaviour : MonoBehaviour
{
    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => ExitScene();
    }

    /// <summary>
    /// when this is called it returns to the previous scene according to scene manager
    /// </summary>
    public void ExitScene()
    {
        if (AframaxSceneManager.Instance.LastSceneIndex == AframaxSceneManager.Instance.GameplaySceneIndex)
        {
            AframaxSceneManager.Instance.RemoveAdditiveLoadedScene(AframaxSceneManager.Instance.SettingsSceneIndex);
        }
        else
        {
            AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(AframaxSceneManager.Instance.LastSceneIndex, 0);
        }
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        _playerInputControls.Player.Pause.performed -= ctx => ExitScene();
        _playerInputControls.Disable();
    }
}
