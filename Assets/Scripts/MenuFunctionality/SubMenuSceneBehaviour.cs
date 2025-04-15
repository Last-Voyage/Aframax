/*****************************************************************************
// File Name :         SubMenuSceneBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     11/10/24
//
// Brief Description : handles using escape to leave the submenu scenes
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// this makes it so u can press escape to return to the previous scene
/// </summary>
public class SubMenuSceneBehaviour : MonoBehaviour
{
    private PlayerInputMap _playerInputControls;

    [SerializeField] private int _thisSceneID;

    [Space]
    [Tooltip("If this scene is not additive, we will transition to a set scene")]
    [SerializeField] private bool _willTransition = false;
    [SerializeField] private int _targetScene = 0;

    [Space]
    [SerializeField] private bool _doesSaveOnExit;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.UIBack.performed += ctx => ExitScene();
    }

    /// <summary>
    /// when this is called it returns to the previous scene according to scene manager
    /// </summary>
    public void ExitScene()
    {
        //checks if this scene was additively loaded. if additive, then just unload it. if not, load old scene or next one
        if (SceneManager.GetActiveScene().buildIndex == _thisSceneID)
        {
            if (_willTransition)
            {
                AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(_targetScene, 0);
            }
            else
            {
                AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(AframaxSceneManager.Instance.LastSceneIndex, 0);
            }
        }
        else
        {
            if(_doesSaveOnExit)
            {
                SaveManager.Instance.SaveText();
            }
            
            AframaxSceneManager.Instance.RemoveAdditiveLoadedScene(_thisSceneID);
        }
    }

    private void OnEnable()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(true);
        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        AframaxSceneManager.Instance.SetSubMenuSceneLoadedBool(false);
        _playerInputControls.Player.UIBack.performed -= ctx => ExitScene();
        _playerInputControls.Disable();
    }
}
