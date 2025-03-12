/*****************************************************************************
// File Name :         PlaceholderTutorialBehaviour.cs
// Author :            Jeremiah Peters
// Creation Date :     3/8/25
//
// Brief Description : placeholder tutorial functionality
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// placeholder functionality for the tutorial, largely taken from NoteInteractable.cs
/// </summary>
public class PlaceholderTutorialBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialPagesContainer;
    private int _currentPage;

    [SerializeField] [Multiline] private string[] _pageTexts;
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private Image _leftArrow;
    [SerializeField] private Image _rightArrow;

    [SerializeField] private bool _isSingleUse;

    public static PlaceholderTutorialBehaviour ActivePlaceholderTutorial = null;

    private PlayerInputMap _playerInputMap;

    /// <summary>
    /// set up controls
    /// </summary>
    private void Awake()
    {
        _playerInputMap = new PlayerInputMap();
        
    }

    /// <summary>
    /// when the player enters range, pop up tutorial
    /// </summary>
    /// <param name="other">whatever collided with this, hopefully the player</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            _tutorialPagesContainer.SetActive(true);

            // Free the mouse and freeze the game
            TimeManager.Instance.GetOnGamePauseEvent()?.Invoke();

            ActivePlaceholderTutorial = this;

            // Reset the page counter to the first page and activate the note
            _currentPage = 0;
            _tutorialPagesContainer.SetActive(true);
            ChangePage(_currentPage);
        }
    }

    /// <summary>
    /// used for cycling between pages of the tutorial
    /// </summary>
    /// <param name="value"></param>
    public void ChangePage(int value)
    {
        // Clamp the page to the bounds of the note, then assign the text
        _currentPage = Mathf.Clamp(_currentPage + value, 0, _pageTexts.Length - 1);
        _textField.text = _pageTexts[_currentPage];

        _leftArrow.color = _currentPage == 0 ? Color.clear : Color.white;
        _rightArrow.color = _currentPage == _pageTexts.Length - 1 ? Color.clear : Color.white;
    }

    /// <summary>
    /// used for exiting the tutorial
    /// </summary>
    public void HidePlaceholderTutorial()
    {
        // Lock the mouse and unfreeze the game
        TimeManager.Instance.GetOnGameUnpauseEvent()?.Invoke();

        ActivePlaceholderTutorial = null;

        // Deactivate the note
        _tutorialPagesContainer.SetActive(false);

        if (_isSingleUse)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// for PauseMenu.cs to use to close the tutorial instead of pausing the game
    /// </summary>
    public static void ExitActiveTutorial()
    {
        ActivePlaceholderTutorial.HidePlaceholderTutorial();
    }

    /// <summary>
    /// Allows the player to use arrow keys or bumpers to change the note page
    /// </summary>
    private void OnEnable()
    {
        _playerInputMap.Enable();
        _playerInputMap.Player.UICycling.performed += ctx => ChangePage((int)ctx.ReadValue<float>());
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        _playerInputMap.Player.UICycling.performed -= ctx => ChangePage((int)ctx.ReadValue<float>());
        _playerInputMap.Disable();
    }
}
