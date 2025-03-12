/*****************************************************************************
// File Name :         NoteInteractable.cs
// Author :            Charlie Polonus
// Contributor:        Nick Rice
// Creation Date :     1/27/25
//
// Brief Description : Controls an interactable note in scene. When
                       interacted with, it opens the note view prefab.
*****************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The MonoBehaviour that manages anything that can be interacted with and read
/// </summary>
public class NoteInteractable : MonoBehaviour, IPlayerInteractable
{
    public static NoteInteractable ActiveNote = null;
    private static ConsoleController _activeConsole = null;

    [SerializeField] [Multiline] private string[] _pageTexts;

    [SerializeField] private GameObject _noteView;
    [SerializeField] private TMP_Text _noteTextField;
    [SerializeField] private Image _leftArrow;
    [SerializeField] private Image _rightArrow;
    [SerializeField] private ScriptableDialogueUi _dialogueOnExit;
    [SerializeField] private UnityEvent _onDialogueExit;
    [SerializeField] private bool _onlyPlayOnce = true;
    private SpriteRenderer _interactablePopUp;
    private bool _hasPlayed;
    private int _currentPage;

    private PlayerInputMap _playerInputMap;

    public bool HasPlayed => _hasPlayed;

    /// <summary>
    /// Attempt to find the console manager in the scene if it hasn't been assigned already
    /// </summary>
    private void Awake()
    {
	    _noteView.transform.parent = null;
	    _noteView.transform.rotation = Quaternion.identity;
        if (_activeConsole == null)
        {
            _activeConsole = FindAnyObjectByType<ConsoleController>();
        }

        _playerInputMap = new PlayerInputMap();

        // Sets the interactable popup to the childed sprite
        _interactablePopUp = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// Change the currently active page by a certain value
    /// </summary>
    /// <param name="value">The amount to change the page by</param>
    public void ChangePage(int value)
    {
        // Clamp the page to the bounds of the note, then assign the text
        _currentPage = Mathf.Clamp(_currentPage + value, 0, _pageTexts.Length - 1);
        _noteTextField.text = _pageTexts[_currentPage];

        _leftArrow.color = _currentPage == 0 ? Color.clear : Color.white;
        _rightArrow.color = _currentPage == _pageTexts.Length - 1 ? Color.clear : Color.white;
    }

    /// <summary>
    /// Show the note through the UI when the player interacts with it
    /// </summary>
    public void OnInteractedByPlayer()
    {
        // Edge cases: There's no notes or something is already open
        if (ActiveNote != null
            || Time.timeScale == 0)
        {
            return;
        }

        ShowNote();
    }

    /// <summary>
    /// Toggle on the note UI
    /// </summary>
    private void ShowNote()
    {
        // Free the mouse and freeze the game
        TimeManager.Instance.GetOnGamePauseEvent()?.Invoke();
        
        // Enables a/d, arrow keys, and shoulder button controls
        _playerInputMap.Enable();
        _playerInputMap.Player.UICycling.performed += ctx => ChangePage((int)ctx.ReadValue<float>());

        // Reset the page counter to the first page and activate the note
        _currentPage = 0;
        ActiveNote = this;
        _noteView.SetActive(true);
        ChangePage(_currentPage);

        // Hide the interaction popup
        _interactablePopUp.enabled = false;
    }

    /// <summary>
    /// Toggle off the note UI
    /// </summary>
    public void HideNote()
    {
        // Edge cases: The console is in use and the console is open
        if (_activeConsole != null
            && _activeConsole.ConsoleIsOpen())
        {
            return;
        }

        // Lock the mouse and unfreeze the game
        TimeManager.Instance.GetOnGameUnpauseEvent()?.Invoke();

        // Deactivate the note
        ActiveNote = null;
        _noteView.SetActive(false);

        if (!_onlyPlayOnce || !_hasPlayed)
        {
            if (_dialogueOnExit != null)
            {
                GameStateManager.Instance.GetOnNewDialogueChain()?.Invoke(_dialogueOnExit);
                
                _hasPlayed = true;
            }
            _onDialogueExit?.Invoke();
        }

        // Show the interaction popup
        _interactablePopUp.enabled = true;
    }

    /// <summary>
    /// Removes the listeners to the event
    /// </summary>
    private void OnDestroy()
    {
        _onDialogueExit?.RemoveAllListeners();
    }

    /// <summary>
    /// Close the currently active note
    /// </summary>
    public static void ExitActiveNote()
    {
        ActiveNote.HideNote();
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        _playerInputMap.Player.UICycling.performed -= ctx =>ChangePage((int)ctx.ReadValue<float>());
        _playerInputMap.Disable();
    }
}
