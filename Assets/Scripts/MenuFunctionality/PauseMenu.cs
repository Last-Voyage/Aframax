/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
// Contributers :      Ryan Swanson, Charlie Polonus, Adam Garwacki
// Creation Date :     9/28/24
//
// Brief Description : operates pausing the game and the pause menu buttons
*****************************************************************************/

using UnityEngine;
using System.Collections;

/// <summary>
/// functionality for pausing the game and the pause menu buttons
/// </summary>
public class PauseMenu : MonoBehaviour
{
    //Contains all ui to toggle on and off
    //Personally I prefer to make this serialized rather than getting the child in awake
    //You can get children by order, but that order can change if messed with
    [SerializeField] private GameObject _pauseMenuContent;

    [Tooltip("The first page of the pause menu")]
    [SerializeField] private GameObject _pauseMainMenu;
    [Tooltip("The second page of the pause menu allowing the player to check settings or controls.")]
    [SerializeField] private GameObject _pauseSubmenu;

    [Space]
    [SerializeField] private SubMenuTextBehaviour _subTextBehaviour;
    [SerializeField] private int _pauseIndexToOpen;
    [SerializeField] private int _pauseIndexToClose;

    [Header("Fields for Loading")]
    [SerializeField] private float _loadInWaitTime;
    [SerializeField] private GameObject _solidScrim;

    private PlayerInputMap _playerInputControls;

    public static PauseMenu Instance;

    private void Awake()
    {
        CheckSingletonInstance();

        StartCoroutine(LoadingBuffer());

        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.Pause.performed += ctx => PauseToggle();
    }

    /// <summary>
    /// Freezes player inputs to give assets a chance to load upon level load
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadingBuffer()
    {
        PauseToggle();
        _solidScrim.SetActive(true);
        yield return new WaitForSecondsRealtime(_loadInWaitTime);
        _solidScrim.SetActive(false);
        PauseToggle();
    }

    /// <summary>
    /// Confirms whether this asset exists as a singleton.
    /// </summary>
    private void CheckSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// toggles the pause state so you can press escape again to close the pause menu
    /// Public so that it can be accessed by button
    /// </summary>
    public void PauseToggle()
    {
        // Toggles pause menu page
        if (!AframaxSceneManager.Instance.IsASubMenuSceneLoaded && !_pauseMainMenu.activeSelf && _pauseSubmenu.activeSelf)
        {
            RevertPausePage();
            return;
        }

        if (TutorialPopUp.ActiveTutorial != null)
        {
            TutorialPopUp.ExitActivePopUp();
        }

        // Exit the note instead of pausing
        if (NoteInteractable.ActiveNote != null)
        {
            NoteInteractable.ExitActiveNote();
        }

        if (PlaceholderTutorialBehaviour.ActivePlaceholderTutorial != null)
        {
            PlaceholderTutorialBehaviour.ExitActiveTutorial();
        }

        //don't unpause if the settings scene is loaded
        if (!AframaxSceneManager.Instance.IsASubMenuSceneLoaded)
        {
            TimeManager.Instance.PauseGameToggle(true);
        }

    }

    /// <summary>
    /// Enables and disables the pause menu ui
    /// </summary>
    /// <param name="isVisible"></param>
    private void PauseUIVisibility(bool isVisible,bool shouldToggleAudio)
    {
        _pauseMenuContent.SetActive(isVisible);
        
        if (isVisible)
        {
            GameStateManager.Instance.GetOnGamePaused()?.Invoke();
        }
        else
        {
            GameStateManager.Instance.GetOnGameUnpaused()?.Invoke();            
        }

        if(shouldToggleAudio)
        {
            // Pauses or resumes all the audio based on whether or not the menu is visible
            FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/In-Game", out FMOD.Studio.Bus masterBus);
            masterBus.setPaused(isVisible);
        }
    }

    /// <summary>
    /// it quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Reverts the pause menu's page to its default one.
    /// </summary>
    private void RevertPausePage()
    {
        _subTextBehaviour.EnableMenuElement(_pauseIndexToOpen);
        _subTextBehaviour.DisableMenuElement(_pauseIndexToClose);

    }

    private void OnEnable()
    {
        _playerInputControls.Enable();

        TimeManager.Instance.GetOnGamePauseToggleEvent().AddListener(PauseUIVisibility);
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();

        TimeManager.Instance.GetOnGamePauseToggleEvent().RemoveListener(PauseUIVisibility);
    }
}
