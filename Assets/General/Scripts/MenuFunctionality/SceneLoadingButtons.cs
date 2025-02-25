/******************************************************************************
// File Name:       SceneLoadingButtons.cs
// Author:          Ryan Swanson
// Creation Date:   September 23, 2024
//
// Description:     Placed on buttons to allow for them to load scenes with specific transitions
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality to buttons to load scenes
/// </summary>
public class SceneLoadingButtons : MonoBehaviour
{
    [SerializeField] private int _sceneToLoad;
    [SerializeField] private int _sceneTransitionID;

    /// <summary>
    /// The function called by the button to load the desired scene
    /// </summary>
    public void LoadDesiredSceneButton()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(_sceneToLoad, _sceneTransitionID);
    }

    /// <summary>
    /// function for buttons to use for additive loading (used for loading the settings menu)
    /// </summary>
    public void LoadDesiredSceneButtonAdditive()
    {
        AframaxSceneManager.Instance.AdditiveLoadScene(_sceneToLoad);
    }

    /// <summary>
    /// This loads the last saved scene
    /// </summary>
    public void LoadSavedScene()
    {
        //Debug.Log(SaveManager.Instance.GetGameSaveData().GetCurrentSceneIndex());
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID
            (SaveManager.Instance.GetGameSaveData().GetCurrentSceneIndex(),_sceneTransitionID);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
