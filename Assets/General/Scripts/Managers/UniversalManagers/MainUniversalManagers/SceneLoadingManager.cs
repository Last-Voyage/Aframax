/******************************************************************************
// File Name:       SceneLoadingManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides the framework to be used by the core managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides the functionality for scenes to be loaded
/// Can be accessed from the universal manager
/// </summary>
public class SceneLoadingManager : MainUniversalManagerFramework
{
    [SerializeField] private List<SceneTransition> _sceneTransitions;
    private Coroutine _sceneLoadingCoroutine;

    /// <summary>
    /// Starts loading the specified scene id using the specified scene transition
    /// </summary>
    /// <param name="sceneID"></param>
    /// <param name="sceneTransition"></param>
    private void StartAsyncSceneLoad(int sceneID, SceneTransition sceneTransition)
    {
        //Only starts loading a scene if no other scene is being loaded already
        if (_sceneLoadingCoroutine == null)
        {
            _sceneLoadingCoroutine = StartCoroutine(AsyncSceneLoadingProcess(sceneID,sceneTransition));
        }
    }

    /// <summary>
    /// The process by which a new scene is async loaded
    /// </summary>
    /// <param name="sceneID"></param>
    /// <param name="sceneTransition"></param>
    /// <returns></returns>
    private IEnumerator AsyncSceneLoadingProcess(int sceneID, SceneTransition sceneTransition)
    {
        //Starts loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);

        //Can start the starting scene transition animation here
        //Will be implemented when scene transition work occurs

        //Waits for a minimum amount of time before  
        yield return new WaitForSeconds(sceneTransition.GetMinimumSceneTransitionTime());

        //Wait until the asynchronous scene fully loads
        //This exists to hide any screen freeze from loading an intense scene
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        //Can start the ending scene transition animation here
        //Will be implemented when scene transition work occurs

        //Sets the coroutine to null to allow for new scene loading to occur
        _sceneLoadingCoroutine = null;
    }

    /// <summary>
    /// Additively loads a specific scene
    /// </summary>
    /// <param name="sceneID"></param>
    private void AdditiveLoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Removes a specific scene from being additively loaded
    /// </summary>
    /// <param name="sceneID"></param>
    private void RemoveAdditiveLoadedScene(int sceneID)
    {
        SceneManager.UnloadSceneAsync(sceneID);
    }

    #region Base Manager
    public override void SetupMainManager()
    {
        base.SetupMainManager();
    }
    #endregion

    #region Getters

    #endregion
}

/// <summary>
/// Provides the data unique to each scene transition
/// </summary>
[System.Serializable]
public class SceneTransition
{
    [SerializeField] private string _sceneTransitionName;
    [SerializeField] private float _minimumScreenTransitionTime;

    /// <summary>
    /// Strings for animation triggers
    /// </summary>
    [Space]
    [SerializeField] private string _sceneTransitionIntroAnimTrigger;
    [SerializeField] private string _sceneTransitionExitAnimTrigger;

    #region Getters
    public string GetSceneTransitionName() => _sceneTransitionName;
    public float GetMinimumSceneTransitionTime() => _minimumScreenTransitionTime;
    #endregion
}

