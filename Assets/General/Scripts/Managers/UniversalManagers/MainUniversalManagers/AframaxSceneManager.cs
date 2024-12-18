/******************************************************************************
// File Name:       AframaxSceneManager.cs
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
public class AframaxSceneManager : MainUniversalManagerFramework
{
    [SerializeField] private List<SceneTransition> _sceneTransitions;
    private Coroutine _sceneLoadingCoroutine;

    [field: SerializeField] public int MainMenuSceneIndex { get; private set; }

    [field: SerializeField] public int GameplaySceneIndex { get; private set; }

    [field: SerializeField] public int DeathScreenSceneIndex { get; private set; }

    [field: SerializeField] public int EndScreenSceneIndex { get; private set; }

    [field: SerializeField] public int SettingsSceneIndex { get; private set; }

    public int LastSceneIndex { get; private set; }

    public bool IsASubMenuSceneLoaded { get; private set; }

    public static AframaxSceneManager Instance;

    //Occurs when the currently active scene is changed
    private readonly UnityEvent _onBeforeSceneChange = new();
    private readonly UnityEvent _onSceneChanged = new();
    private readonly UnityEvent _onGameplaySceneLoaded = new();
    private readonly UnityEvent _onLeavingGameplayScene = new();

    private readonly UnityEvent _onEndOfGameScene = new();

    private readonly UnityEvent _onAdditiveLoadAddedEvent = new();
    private readonly UnityEvent _onAdditiveLoadRemovedEvent = new();

    private bool _isGameplaySceneLoaded;

    /// <summary>
    /// Subscribes to any needed gameplay events
    /// </summary>
    protected override void SubscribeToGameplayEvents()
    {
        base.SubscribeToGameplayEvents();
        _isGameplaySceneLoaded = true;
        PlayerManager.Instance.GetOnPlayerDeath().AddListener(LoadDeathScreen);
    }

    /// <summary>
    /// Unsubscribes to any subscribed gameplay events
    /// </summary>
    protected override void UnsubscribeToGameplayEvents()
    {
        base.UnsubscribeToGameplayEvents();
        _isGameplaySceneLoaded = false;
        PlayerManager.Instance.GetOnPlayerDeath().RemoveListener(LoadDeathScreen);
    }

    /// <summary>
    /// Checks if the current scene is a game scene or auxiliary scene
    /// </summary>
    /// <returns> True if currently in a game scene </returns>
    public bool IsGameScene()
    {
        return !(SceneManager.GetActiveScene().buildIndex == MainMenuSceneIndex ||
               SceneManager.GetActiveScene().buildIndex == DeathScreenSceneIndex ||
               SceneManager.GetActiveScene().buildIndex == EndScreenSceneIndex ||
               SceneManager.GetActiveScene().buildIndex == SettingsSceneIndex);
    }

    /// <summary>
    /// Starts loading the specified scene id using the specified scene transition
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to load</param>
    /// <param name="sceneTransition">The scene transition being used</param>
    private void StartAsyncSceneLoad(int sceneID, SceneTransition sceneTransition)
    {
        UpdateLastScene();

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
    private void LoadDeathScreen()
    {
        StartAsyncSceneLoadViaID(DeathScreenSceneIndex, 0);
    }
    
    /// <summary>
    /// Loads the end scene for when the player "wins"
    /// </summary>
    public void LoadEndScene()
    {
        InvokeEndOfGameScene();
        StartAsyncSceneLoadViaID(EndScreenSceneIndex, 0);
    }

    /// <summary>
    /// The process by which a new scene is async loaded
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to load</param>
    /// <param name="sceneTransition">The scene transition being used</param>
    /// <returns></returns>
    private IEnumerator AsyncSceneLoadingProcess(int sceneID, SceneTransition sceneTransition)
    {
        InvokeOnBeforeSceneChangeEvent();

        if (_isGameplaySceneLoaded)
        {
            InvokeOnLeavingGameplayScene();
        }
        
        //Starts loading the scene
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneID);

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
    public void AdditiveLoadScene(int sceneID)
    {
        UpdateLastScene();
        SceneManager.LoadScene(sceneID, LoadSceneMode.Additive);

        InvokeOnSceneAdditiveLoadAddEvent();
    }

    /// <summary>
    /// Removes a specific scene from being additively loaded
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to remove</param>
    public void RemoveAdditiveLoadedScene(int sceneID)
    {
        SceneManager.UnloadSceneAsync(sceneID);

        InvokeOnSceneAdditiveLoadRemoveEvent();
    }

    /// <summary>
    /// updates the last scene index to the currently loaded scene. called before loading a new scene
    /// </summary>
    private void UpdateLastScene()
    {
        LastSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// toggles the bool value for checking if the settings scene is loaded
    /// </summary>
    public void SetSubMenuSceneLoadedBool(bool value)
    {
        IsASubMenuSceneLoaded = value;
    }

    #region Base Manager
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    #endregion

    #region Events
    /// <summary>
    /// Invokes event for just before a scene changes
    /// </summary>
    private void InvokeOnBeforeSceneChangeEvent()
    {
        _onBeforeSceneChange?.Invoke();
    }

    /// <summary>
    /// Invokes event for after a scene changes
    /// </summary>
    private void InvokeOnSceneChangedEvent()
    {
        _onSceneChanged?.Invoke();
    }

    /// <summary>
    /// Invokes event for when a gameplay scene is loaded
    /// A gameplay scene is a scene with gameplay managers
    /// </summary>
    public void InvokeOnGameplaySceneLoaded()
    {
        _onGameplaySceneLoaded?.Invoke();
    }

    /// <summary>
    /// Invokes event for when leaving a gameplay scene
    /// </summary>
    public void InvokeOnLeavingGameplayScene()
    {
        _onLeavingGameplayScene?.Invoke();
    }

    /// <summary>
    /// Invokes an event for when a scene is additively loaded
    /// </summary>
    private void InvokeOnSceneAdditiveLoadAddEvent()
    {
        _onAdditiveLoadAddedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes an event for when an additively loaded scene is removed
    /// </summary>
    private void InvokeOnSceneAdditiveLoadRemoveEvent()
    {
        _onAdditiveLoadRemovedEvent?.Invoke();
    }

    /// <summary>
    /// For Boss Attacks Act System to end the game
    /// </summary>
    public void InvokeEndOfGameScene()
    {
        _onEndOfGameScene?.Invoke();
    }

    #endregion

    #region Getters

    public UnityEvent GetOnBeforeSceneChanged => _onBeforeSceneChange;

    public UnityEvent GetOnSceneChanged => _onSceneChanged;

    public UnityEvent GetOnGameplaySceneLoaded => _onGameplaySceneLoaded;

    public UnityEvent GetOnLeavingGameplayScene => _onLeavingGameplayScene;

    public UnityEvent GetOnEndOfGameScene => _onEndOfGameScene;

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
