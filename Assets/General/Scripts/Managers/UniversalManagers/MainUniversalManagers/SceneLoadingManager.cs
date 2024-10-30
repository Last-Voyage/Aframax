/******************************************************************************
// File Name:       SceneLoadingManager.cs
// Author:          Ryan Swanson
// Contributor:     Jeremiah Peters
// Creation Date:   September 15, 2024
//
// Description:     Provides the framework to be used by the core managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality for scenes to be loaded
/// Can be accessed from the universal manager
/// </summary>
public class SceneLoadingManager : MainUniversalManagerFramework
{
    [SerializeField] private List<SceneTransition> _sceneTransitions;
    private Coroutine _sceneLoadingCoroutine;

    [field: SerializeField] public int MainMenuSceneIndex { get; private set; }

    [field: SerializeField] public int DeathScreenSceneIndex { get; private set; }

    public static SceneLoadingManager Instance;

    //Occurs when the currently active scene is changed
    private UnityEvent _onSceneChangedEvent = new();
    private UnityEvent _onGameplaySceneLoaded = new();

    private UnityEvent _onAdditiveLoadAddedEvent = new();
    private UnityEvent _onAdditiveLoadRemovedEvent = new();

    private void Awake()
    {
        SetupInstance();
        SubscribeToEvents();
    }

    /// <summary>
    /// Starts loading the specified scene id using the specified scene transition
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to load</param>
    /// <param name="sceneTransition">The scene transition being used</param>
    private void StartAsyncSceneLoad(int sceneID, SceneTransition sceneTransition)
    {
        //Only starts loading a scene if no other scene is being loaded already
        if (_sceneLoadingCoroutine == null)
        {
            _sceneLoadingCoroutine = StartCoroutine(AsyncSceneLoadingProcess(sceneID,sceneTransition));
        }
    }

    /// <summary>
    /// Starts loading the specified scene using the id of the scene and id of the transition
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to load</param>
    /// <param name="sceneTransitionID">The id of the scene transition that is being used</param>
    public void StartAsyncSceneLoadViaID(int sceneID, int sceneTransitionID)
    {
        StartAsyncSceneLoad(sceneID, _sceneTransitions[sceneTransitionID]);
    }

    /// <summary>
    /// This will be changed to happening through a button later, so this function is temporary
    /// </summary>
    public void LoadDeathScreen()
    {
        StartAsyncSceneLoadViaID(SceneLoadingManager.Instance.DeathScreenSceneIndex, 0);
    }

    /// <summary>
    /// The process by which a new scene is async loaded
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to load</param>
    /// <param name="sceneTransition">The scene transition being used</param>
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

        InvokeOnSceneChangedEvent();

        //Can start the ending scene transition animation here
        //Will be implemented when scene transition work occurs

        //Sets the coroutine to null to allow for new scene loading to occur
        _sceneLoadingCoroutine = null;
    }

    /// <summary>
    /// Additively loads a specific scene
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to add</param>
    private void AdditiveLoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID, LoadSceneMode.Additive);

        InvokeOnSceneAdditiveLoadAddEvent();
    }

    /// <summary>
    /// Removes a specific scene from being additively loaded
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to remove</param>
    private void RemoveAdditiveLoadedScene(int sceneID)
    {
        SceneManager.UnloadSceneAsync(sceneID);

        InvokeOnSceneAdditiveLoadRemoveEvent();
    }

    #region Base Manager
    public override void SetupInstance()
    {
        base.SetupInstance();
        Instance = this;
    }

    public override void SetupMainManager()
    {
        base.SetupMainManager();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    /// <summary>
    /// Subscribes to all events relating to gameplay
    /// This is only called when loading into a scene with Gameplay Managers in it
    /// Subscribes to events that come from gameplay manager
    /// </summary>
    protected override void SubscribeToGameplayEvents()
    {
        base.SubscribeToGameplayEvents();
        PlayerManager.Instance.GetOnPlayerDeath().AddListener(LoadDeathScreen);
    }
    #endregion

    #region Events
    /// <summary>
    /// Invokes event associated with loading a new scene
    /// </summary>
    private void InvokeOnSceneChangedEvent()
    {
        _onSceneChangedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event associated with loading a gameplay scene
    /// Gameplay scenes are any scenes containing gameplay managers
    /// </summary>
    public void InvokeOnGameplaySceneLoaded()
    {
        _onGameplaySceneLoaded?.Invoke();
    }

    /// <summary>
    /// Invokes event associated with adding an additively loaded scene
    /// </summary>
    private void InvokeOnSceneAdditiveLoadAddEvent()
    {
        _onAdditiveLoadAddedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes event associated with removing an additively loaded scene
    /// </summary>
    private void InvokeOnSceneAdditiveLoadRemoveEvent()
    {
        _onAdditiveLoadRemovedEvent?.Invoke();
    }

    #endregion

    #region Getters

    public UnityEvent GetOnSceneChangedEvent() => _onSceneChangedEvent;

    public UnityEvent GetGameplaySceneLoaded() => _onGameplaySceneLoaded;

    public UnityEvent GetOnAdditiveSceneAdded() => _onAdditiveLoadAddedEvent;
    public UnityEvent GetOnAdditiveSceneRemoved() => _onAdditiveLoadRemovedEvent;

    #endregion
}

/// <summary>
/// Provides the data unique to each scene transition
/// </summary>
[System.Serializable]
public struct SceneTransition
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
