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

public class SettingsSceneBehaviour : MonoBehaviour
{
    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => ExitScene();
    }

    public void ExitScene()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(AframaxSceneManager.Instance._lastSceneIndex, 0);
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();
    }
}
