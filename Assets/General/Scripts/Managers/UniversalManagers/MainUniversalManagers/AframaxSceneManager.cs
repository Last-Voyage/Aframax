/******************************************************************************
// File Name:       AframaxSceneManager.cs
// Author:          Ryan Swanson
// Contributor:     Jeremiah Peters, Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Provides the framework to be used by the core managers
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Provides the functionality for scenes to be loaded
/// Can be accessed from the universal manager
/// </summary>
public class AframaxSceneManager : MainUniversalManagerFramework
{
    [SerializeField] private List<SceneTransition> _sceneTransitions;
    private Coroutine _sceneLoadingCoroutine;

    [field: SerializeField] public int MainMenuSceneIndex { get; private set; }

    [field: SerializeField] public int BoatSceneIndex { get; private set; }
    
    [field: SerializeField] public int MazeSceneIndex { get; private set; }

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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return !(currentSceneIndex == MainMenuSceneIndex ||
               currentSceneIndex == DeathScreenSceneIndex ||
               currentSceneIndex == EndScreenSceneIndex ||
               currentSceneIndex == SettingsSceneIndex ||
               currentSceneIndex == MazeSceneIndex);
    }

    /// <summary>
    /// Checks if you are currently above deck
    /// </summary>
    /// <returns> True if in an above deck scene </returns>
    public bool IsAboveDeck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        return (currentSceneIndex == BoatSceneIndex);
    }

    /// <summary>
    /// Checks if you are currently below deck
    /// </summary>
    /// <returns> True if in a below deck scene </returns>
    public bool IsBelowDeck()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        return (currentSceneIndex == MazeSceneIndex);
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
        OnInvokeEndOfGameScene();
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
        OnInvokeBeforeSceneChangeEvent();

        if (_isGameplaySceneLoaded)
        {
            OnInvokeLeavingGameplayScene();
        }

        //start the scene transition animation here
        if (sceneTransition.SceneTransitionIntroAnimTrigger != "")
        {
            SceneTransitionBehaviour.Instance.PlayTransition(sceneTransition.SceneTransitionIntroAnimTrigger);
        }

        //turn off buttons to prevent doing stuff during transition
        GameObject.Find("EventSystem").GetComponent<EventSystem>().enabled = false;

        //Waits for a minimum amount of time before  
        yield return new WaitForSeconds(sceneTransition.GetMinimumSceneTransitionTime());

        //Starts loading the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);

        //Wait until the asynchronous scene fully loads
        //This exists to hide any screen freeze from loading an intense scene
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        OnInvokeSceneChangedEvent();

        //start the ending scene transition animation here
        if (sceneTransition.SceneTransitionIntroAnimTrigger != "")
        {
            SceneTransitionBehaviour.Instance.PlayTransition(sceneTransition.SceneTransitionExitAnimTrigger);
        }

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

        OnInvokeSceneAdditiveLoadAddEvent();
    }

    /// <summary>
    /// Removes a specific scene from being additively loaded
    /// </summary>
    /// <param name="sceneID">The specific scene in the build index to remove</param>
    public void RemoveAdditiveLoadedScene(int sceneID)
    {
        SceneManager.UnloadSceneAsync(sceneID);

        OnInvokeSceneAdditiveLoadRemoveEvent();
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
    private void OnInvokeBeforeSceneChangeEvent()
    {
        _onBeforeSceneChange?.Invoke();
    }

    /// <summary>
    /// Invokes event for after a scene changes
    /// </summary>
    private void OnInvokeSceneChangedEvent()
    {
        _onSceneChanged?.Invoke();
    }

    /// <summary>
    /// Invokes event for when a gameplay scene is loaded
    /// A gameplay scene is a scene with gameplay managers
    /// </summary>
    public void OnInvokeGameplaySceneLoaded()
    {
        _onGameplaySceneLoaded?.Invoke();
    }

    /// <summary>
    /// Invokes event for when leaving a gameplay scene
    /// </summary>
    public void OnInvokeLeavingGameplayScene()
    {
        _onLeavingGameplayScene?.Invoke();
    }

    /// <summary>
    /// Invokes an event for when a scene is additively loaded
    /// </summary>
    private void OnInvokeSceneAdditiveLoadAddEvent()
    {
        _onAdditiveLoadAddedEvent?.Invoke();
    }

    /// <summary>
    /// Invokes an event for when an additively loaded scene is removed
    /// </summary>
    private void OnInvokeSceneAdditiveLoadRemoveEvent()
    {
        _onAdditiveLoadRemovedEvent?.Invoke();
    }

    /// <summary>
    /// For Boss Attacks Act System to end the game
    /// </summary>
    public void OnInvokeEndOfGameScene()
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
    public string SceneTransitionIntroAnimTrigger;
    public string SceneTransitionExitAnimTrigger;

    #region Getters
    public string GetSceneTransitionName() => _sceneTransitionName;
    public float GetMinimumSceneTransitionTime() => _minimumScreenTransitionTime;
    #endregion
}
