/*****************************************************************************
// File Name :         TutorialPopUp.cs
// Author :            Charlie Polonus
// Contributors :      Adam Garwacki
// Creation Date :     3/2/25
//
// Brief Description : Controls a tutorial pop up in-engine.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

/// <summary>
/// A collection of pages for a popup tutorial
/// </summary>
public class TutorialPopUp : MonoBehaviour, IPlayerInteractable, IUiSwap
{
    public static TutorialPopUp ActiveTutorial = null;

    [Header("References")]
    [SerializeField] private Canvas _popupCanvas;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private Transform _pageParent;
    [Space]
    [SerializeField] private UnityEvent _onDialogueExit;
    private GameObject[] _pages;
    private int _currentPage;
    private PlayerInputMap _playerInputMap;
    private bool _hasDoorOpened;
    private InGameMenuSwap _menuSwapScript;

    [SerializeField] private ButtonSFXManager _buttonSFXManagerReference;
    
    [SerializeField] private Image _rightPageButton;
    [SerializeField] private Image _leftPageButton;
    [Header("Keyboard UI assets")]
    [SerializeField] private Sprite _keyboardLeftPageButtonAsset, _keyboardRightPageButtonAsset;
    [Header("Controller UI assets")]
    [SerializeField] private Sprite _controllerLeftPageButtonAsset, _controllerRightPageButtonAsset;

    [Space]
    [SerializeField] private bool _doesInteractOnStart = false;

    /// <summary>
    /// Setup the pages list to hold all the possible pages
    /// </summary>
    private void Start()
    {
        _playerInputMap = new PlayerInputMap();

        _pages = new GameObject[_pageParent.childCount];

        _menuSwapScript = _popupCanvas.GetComponent<InGameMenuSwap>();

        for (int i = 0; i < _pageParent.childCount; i++)
        {
            _pages[i] = _pageParent.GetChild(i).gameObject;
        }

        if(_doesInteractOnStart)
        {
            OnInteractedByPlayer();
        } 
    }

    /// <summary>
    /// Open the tutorial pop up and go to page one
    /// </summary>
    public void OpenTutorialPopUp()
    {
        _popupCanvas.enabled = true;

        if(!_doesInteractOnStart)
        {
            // Free the mouse and freeze the game
            TimeManager.Instance.GetOnGamePauseEvent()?.Invoke();

            // I believe that making this true pauses audio, if we want to change that, then it's right below here
            TimeManager.Instance.PauseGameToggle(false);
        }
        
        // Enables a/d, arrow keys, and shoulder button controls
        _playerInputMap.Enable();
        _playerInputMap.Player.UICycling.performed += ctx => ChangePage((int)ctx.ReadValue<float>());

        // Reset the page counter to the first page and activate the note
        _currentPage = 0;
        ActiveTutorial = this;
        _menuSwapScript.DeselectMenu();
        ChangePage(_currentPage);
        
        // Changes the note visuals
        OnUiSwap();
    }

    /// <summary>
    /// Change the currently open page
    /// </summary>
    /// <param name="pageChangeAmount">The value to change the page by</param>
    public void ChangePage(int pageChangeAmount)
    {
        // Stop the currently active page from playing
        StopPage(_pages[_currentPage]);

        // Clamp the page to the bounds of the note, then assign the text
        int _nextPage = Mathf.Clamp(_currentPage + pageChangeAmount, 0, _pages.Length - 1);

        //play sfx if changing page
        if (_currentPage != _nextPage)
        {
            _buttonSFXManagerReference.PlayClickSFX();
        }

        _currentPage = _nextPage;

        // Set the visibility of each page based on the current active page
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(i == _currentPage);
        }

        // Start the newly active page playing
        StartPage(_pages[_currentPage]);

        // Update the arrows to look the correct color
        _leftArrow.interactable = _currentPage != 0;
        _rightArrow.interactable = _currentPage != _pages.Length - 1;
    }

    /// <summary>
    /// Cancel the page's video from playing
    /// </summary>
    /// <param name="page">The selected page</param>
    private void StopPage(GameObject page)
    {
        // Find the video in the page
        VideoPlayer pageVideo = page.GetComponentInChildren<VideoPlayer>();

        // Stop the video if there is one
        if (!pageVideo.clip.IsUnityNull())
        {
            pageVideo.time = 0;
            pageVideo.Stop();
        }
    }

    /// <summary>
    /// Start the page's video playing
    /// </summary>
    /// <param name="page">The selected page</param>
    private void StartPage(GameObject page)
    {
        // Find the video in the page
        VideoPlayer pageVideo = page.GetComponentInChildren<VideoPlayer>();
        RawImage pageVideoImage = page.GetComponentInChildren<RawImage>();
        Image pageImage = page.GetComponentInChildren<Image>();

        bool hasVideo = !pageVideo.clip.IsUnityNull();

        // Play the video if there is one
        if (hasVideo)
        {
            pageVideo.targetCamera = Camera.current;
            pageVideo.time = 0;
            pageVideo.Play();

            pageImage.enabled = false;
        }
        else
        {
            pageVideoImage.enabled = false;
        }
    }

    /// <summary>
    /// Close the popup of the current tutorial object
    /// </summary>
    private void CloseTutorialPopUp()
    {
        // Set the page to inactive and read
        ActiveTutorial = null;
        _popupCanvas.enabled = false;

        // Stop the currently open page
        StopPage(_pages[_currentPage]);

        // Stop accepting A&D/Controller UI input
        _playerInputMap.Player.UICycling.performed -= ctx => ChangePage((int)ctx.ReadValue<float>());
        _playerInputMap.Disable();

        // Set all the pages to off
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(false);
        }

        if (!_hasDoorOpened)
        {
            _onDialogueExit?.Invoke();
            _hasDoorOpened = true;
        }
        
        // Free the mouse and freeze the game
        TimeManager.Instance.GetOnGameUnpauseEvent();
    }

    /// <summary>
    /// Exit the currently active tutorial pop up
    /// </summary>
    public static void ExitActivePopUp()
    {
        ActiveTutorial.CloseTutorialPopUp();
    }

    /// <summary>
    /// Closes the popup of the current tutorial object.
    /// Accessed when clicking an Escape button prompt.
    /// </summary>
    public void ExitActivePopupViaClick()
    {
        PauseMenu.Instance.PauseToggle();
        CloseTutorialPopUp();
    }


    /// <summary>
    /// Override for the player interacting with the tutorial
    /// </summary>
    public void OnInteractedByPlayer()
    {
        // Edge cases: There's no tutorials or something is already open
        if (!ActiveTutorial.IsUnityNull()
            || Time.deltaTime == 0)
        {
            return;
        }

        OpenTutorialPopUp();
    }
    
    /// <summary>
    /// Swaps the left and right page movement sprites
    /// </summary>
    public void OnUiSwap()
    {
        if (UiManager.IsUsingController)
        {
            _leftPageButton.sprite = _controllerLeftPageButtonAsset;
            _rightPageButton.sprite = _controllerRightPageButtonAsset;
        }
        else
        {
            _leftPageButton.sprite = _keyboardLeftPageButtonAsset;
            _rightPageButton.sprite = _keyboardRightPageButtonAsset;
        }
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        _playerInputMap.Player.UICycling.performed -= ctx => ChangePage((int)ctx.ReadValue<float>());
        _playerInputMap.Disable();
    }

    /// <summary>
    /// Removes the listeners to the event
    /// </summary>
    private void OnDestroy()
    {
        _onDialogueExit?.RemoveAllListeners();
    }
}
