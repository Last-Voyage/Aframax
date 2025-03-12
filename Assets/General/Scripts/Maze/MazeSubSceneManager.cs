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

    private Rigidbody _playerRigidbody;
    private bool _firstMazeLoaded = false;

    private int _currentMaze = -1;
    private int _preloadedMaze = -1;

    private IEnumerator _cachedCoroutineToDestroy;
    
    private void Start()
    {
        // Throttle speed of maze loading
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        
        // Get singleton reference to AframaxSceneManager
        _sceneManager = AframaxSceneManager.Instance;
        
        // Subscribe to death event
        PlayerManager.Instance.GetOnPlayerDeath().AddListener(OnPlayerDeath);
        
        // Disable player Rigidbody until scene is loaded
        _playerRigidbody = PlayerMovementController.Instance
            .GetComponent<Rigidbody>();
        _playerRigidbody.isKinematic = true;
        
        // Load first maze
        PreLoadMazeScene(_firstMazeIndex);
        LoadMazeAdditive(_firstMazeIndex);

        FindObjectOfType<SaveReconfiguration>().LoadSave(this);
    }

    private void OnPlayerDeath()
    {
        // Remove blocking operations
        foreach (var asyncOp in _asyncOpList)
        {
            if (asyncOp.Value != null)
            {
                asyncOp.Value.allowSceneActivation = true;
            }
        }
        
        // Stop all coroutines in this behavior
        StopAllCoroutines();
        
        // Delete this component
        Destroy(this);
    }

    /// <summary>
    /// Display the currently loaded maze
    /// </summary>
    public void LoadMazeAdditive(int mazeId)
    {
        _currentMaze = mazeId;
        
        int sceneId = _sceneManager.MazeAdditiveSceneIndices[mazeId];
        
        print("Loaded scene additive, maze part 0"+sceneId+" "+mazeId);
        
        StartCoroutine(StartAsyncSceneLoadOperation(mazeId, sceneId));
    }

    // Coroutine for LoadMazeAdditive()
    private IEnumerator StartAsyncSceneLoadOperation(int mazeId, int sceneId)
    {
        // Load in scene as fast as possible
        Application.backgroundLoadingPriority = ThreadPriority.High;
        
        print("It somehow got past load maze additive in sub scene maze "+mazeId);
        
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
        
        print("It loaded in the scene" +mazeId);
        
        // Enable scene in level
        _asyncOpList[mazeId].allowSceneActivation = true;
        
        // Reset thread priority
        Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
        
        yield break;
    }

    /// <summary>
    /// Load the desired maze SubScene and disable it until it is ready to be used
    /// </summary>
    /// <param name="mazeId">The maze index to be preloaded.</param>
    public void PreLoadMazeScene(int mazeId)
    {
        _preloadedMaze = mazeId;
        
        // (Re)initialize scene loading state
        _loadSceneState = new();
        _loadSceneState.SceneBuildID = AframaxSceneManager.Instance.MazeAdditiveSceneIndices[
            mazeId
        ];
        _loadSceneState.LoadingComplete = false;
        
        print("This is the preload"+mazeId);

        _cachedCoroutineToDestroy = StartSubscenePreLoadOperation(mazeId, _loadSceneState.SceneBuildID);
        StartCoroutine(_cachedCoroutineToDestroy);
    }

    // Coroutine for PreLoadMazeScene()
    private IEnumerator StartSubscenePreLoadOperation(int mazeId, int sceneId)
    {
        print("This is the only other place preload"+mazeId);
        // Check if scene is already preloaded
        if (_asyncOpList.ContainsKey(mazeId))
        {
            yield break;
        }

        // Start async operation
        _asyncOpList.Add(mazeId, SceneManager.LoadSceneAsync(
            sceneId,
            LoadSceneMode.Additive
        ));
        Debug.Assert(_asyncOpList[mazeId] != null, nameof(List<AsyncOperation>) + " != null");

        // Make sure the scene isn't shown until LoadMazeAdditive() is called
        _asyncOpList[mazeId].allowSceneActivation = false; // This isn't working properly apparently

        print("Heyo we got a maze in here");
        
        // Await scene loading
        while (_asyncOpList[mazeId] is { isDone: false } || _asyncOpList[mazeId] is {allowSceneActivation:false}) 
        {
            yield return null;
        }
        
        print("The great maze escape"+mazeId); // It ignores this completely, and yet it still goes through!!!!!!!
        
        // Enable player Rigidbody if running for the first time
        if (!_firstMazeLoaded)
        {
            _playerRigidbody.isKinematic = false;
            _firstMazeLoaded = true;
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

        if (!sceneRef.isLoaded)
        {
            yield break;
        }
        
        AsyncOperation unloadAsyncOp = SceneManager.UnloadSceneAsync(
            sceneRef
        );
        
        // Wait for scene to unload
        while (unloadAsyncOp is { isDone: false })
        {
            yield return null;
        }

        VerifySceneIntegrity();
    }

    /// <summary>
    /// Ensure no scenes but the currently loaded one and
    /// the preloaded one exist in the scene.
    /// </summary>
    public void VerifySceneIntegrity()
    {
        int mazeCount = _sceneManager.MazeAdditiveSceneIndices.Count;
        
        for (int i = 0; i < mazeCount; i++)
        {
            if (i != _currentMaze && i != _preloadedMaze)
            {
                int sceneId = _sceneManager.MazeAdditiveSceneIndices[i];
                StartDestroySceneOperation(sceneId);
            }
        }
    }

    /// <summary>
    /// Th
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This should make sure it only affects improperly loaded scenes
        if (ShouldDisableScene(scene))
        {
            AframaxSceneManager.Instance.RemoveAdditiveLoadedScene(scene.buildIndex);
            StopCoroutine(_cachedCoroutineToDestroy);
            _asyncOpList.Remove(_currentMaze + 1);
            //PreLoadMazeScene(_currentMaze +1);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if ((scene.buildIndex == 4 || scene.buildIndex == 6) && _currentMaze < AframaxSceneManager.Instance.MazeAdditiveSceneIndices.Count-1)
        {
            PreLoadMazeScene(_currentMaze+1);
        }
    }

    /// <summary>
    /// This will check if the 
    /// </summary>
    /// <param name="scene">The new scene being loaded</param>
    /// <returns></returns>
    private bool ShouldDisableScene(Scene scene)
    {
        int howManyMazes = AframaxSceneManager.Instance.MazeAdditiveSceneIndices.Count;
        /*return !AframaxSceneManager.Instance.MazeAdditiveSceneIndices.Contains(scene.buildIndex) &&
               scene.buildIndex != 1 && _currentMaze < howManyMazes-1;*/
        return (scene.buildIndex % 10) != _currentMaze && 
               AframaxSceneManager.Instance.MazeAdditiveSceneIndices.Contains(scene.buildIndex);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
