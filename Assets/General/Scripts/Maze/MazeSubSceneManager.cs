/******************************************************************************
// File Name:       MazeSubSceneManager.cs
// Author:          Miles Rogers
// Contributor:     ...
// Creation Date:   March 3rd, 2025
//
// Description:     Interacts with AframaxSceneManager to additively load
//                  and display scenes
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;

public class MazeSubSceneManager : MonoBehaviour
{
    /// <summary>
    /// The first maze to be loaded into the game world
    /// </summary>
    [SerializeField] private int _firstMazeIndex = 0;

    /// <summary>
    /// Struct that holds the current state of the preloaded
    /// scene.
    /// </summary>
    [Serializable] public struct LoadSceneState
    {
        public int SceneBuildID;
        public bool LoadingComplete;
    }

    private AframaxSceneManager _sceneManager;
    private LoadSceneState _loadSceneState;
    private Dictionary<int, AsyncOperation> _asyncOpList = new();
    
    private void Start()
    {
        // Get singleton reference to AframaxSceneManager
        _sceneManager = AframaxSceneManager.Instance;
        
        // Load first maze
        PreLoadMazeScene(_firstMazeIndex);
        LoadMazeAdditive(_firstMazeIndex);
    }

    /// <summary>
    /// Display the currently loaded maze
    /// </summary>
    public void LoadMazeAdditive(int mazeId)
    {
        int sceneId = _sceneManager.MazeAdditiveSceneIndices[mazeId];
        StartCoroutine(StartAsyncSceneLoadOperation(mazeId, sceneId));
    }

    // Coroutine for LoadMazeAdditive()
    private IEnumerator StartAsyncSceneLoadOperation(int mazeId, int sceneId)
    {
        // Error if no scene is preloaded
        if (_asyncOpList[mazeId] == null)
        {
            throw new NullReferenceException(
                "Scene '" + SceneManager.GetSceneByBuildIndex(sceneId).name + 
                "' is not currently preloaded. Please ensure you've called " +
                "PreLoadMazeScene() with the proper maze index before attempting " +
                "to call LoadNextMazeAdditive()."
            );
        }
        
        // Enable scene in level
        _asyncOpList[mazeId].allowSceneActivation = true;
        
        yield break;
    }

    /// <summary>
    /// Load the desired maze SubScene and disable it until it is ready to be used
    /// </summary>
    /// <param name="mazeId">The maze index to be preloaded.</param>
    public void PreLoadMazeScene(int mazeId)
    {
        // (Re)initialize scene loading state
        _loadSceneState = new();
        _loadSceneState.SceneBuildID = AframaxSceneManager.Instance.MazeAdditiveSceneIndices[
            mazeId
        ];
        _loadSceneState.LoadingComplete = false;

        StartCoroutine(StartSubscenePreLoadOperation(mazeId, _loadSceneState.SceneBuildID));
    }

    // Coroutine for PreLoadMazeScene()
    private IEnumerator StartSubscenePreLoadOperation(int mazeId, int sceneId)
    {
        // Start async operation
        _asyncOpList.Add(mazeId, SceneManager.LoadSceneAsync(
            sceneId,
            LoadSceneMode.Additive
        ));
        Debug.Assert(_asyncOpList[mazeId] != null, nameof(List<AsyncOperation>) + " != null");
        
        // Make sure the scene isn't shown until LoadMazeAdditive() is called
        _asyncOpList[mazeId].allowSceneActivation = false;

        // Await scene loading
        while (_asyncOpList[mazeId] is { isDone: false })
        {
            yield return null;
        }
        
        // Ref to preloaded scene
        var preloadedSceneRef = SceneManager.GetSceneByBuildIndex(sceneId);
        
        // Tell LoadSceneState loading is done
        _loadSceneState.LoadingComplete = true;
    }

    /// <summary>
    /// Destroy maze/unload data context
    /// </summary>
    /// <param name="mazeId">The maze index to be destroyed.</param>
    public void DestroyMaze(int mazeId)
    {
        int sceneId = _sceneManager.MazeAdditiveSceneIndices[mazeId];
        StartCoroutine(StartDestroySceneOperation(sceneId));
    }

    // Coroutine for DestroyMaze()
    private IEnumerator StartDestroySceneOperation(int sceneId)
    {
        var sceneRef = SceneManager.GetSceneByBuildIndex(sceneId);
        
        AsyncOperation unloadAsyncOp = SceneManager.UnloadSceneAsync(
            sceneRef
        );
        
        // Wait for scene to unload
        while (unloadAsyncOp is { isDone: false })
        {
            yield return null;
        }
    }
}
