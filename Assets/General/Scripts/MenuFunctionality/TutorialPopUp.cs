/*****************************************************************************
// File Name :         TutorialPopUp.cs
// Author :            Charlie Polonus
// Creation Date :     3/2/25
//
// Brief Description : Controls a tutorial pop up in-engine.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A collection of pages for a popup tutorial
/// </summary>
public class TutorialPopUp : MonoBehaviour
{
    public static TutorialPopUp ActiveTutorial = null;

    [Header("References")]
    [SerializeField] private Canvas _popupCanvas;
    [SerializeField] private GameObject[] _pages;
    [SerializeField] private TMP_Text _leftArrow;
    [SerializeField] private TMP_Text _rightArrow;
    private int _currentPage;
    private bool _hasRead;

    /// <summary>
    /// Open the tutorial pop up and go to page one
    /// </summary>
    public void OpenTutorialPopUp()
    {
        _popupCanvas.enabled = true;

        // Free the mouse and freeze the game
        TimeManager.Instance.GetOnGamePauseEvent();

        // Reset the page counter to the first page and activate the note
        _currentPage = 0;
        ActiveTutorial = this;
        ChangePage(_currentPage);
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
        _currentPage = Mathf.Clamp(_currentPage + pageChangeAmount, 0, _pages.Length - 1);

        // Set the visibility of each page based on the current active page
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(i == _currentPage);
        }

        // Start the newly active page playing
        StartPage(_pages[_currentPage]);

        // Update the arrows to look the correct color
        _leftArrow.color = _currentPage == 0 ? Color.clear : Color.white;
        _rightArrow.color = _currentPage == _pages.Length - 1 ? Color.clear : Color.white;
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
        if (pageVideo.clip != null)
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

        // Play the video if there is one
        if (pageVideo.clip != null)
        {
            pageVideo.targetCamera = Camera.current;
            pageVideo.time = 0;
            pageVideo.Play();
        }
    }

    /// <summary>
    /// Close the popup of the current tutorial object
    /// </summary>
    private void CloseTutorialPopUp()
    {
        // Set the page to inactive and read
        ActiveTutorial = null;
        _hasRead = true;
        _popupCanvas.enabled = false;

        // Stop the currently open page
        StopPage(_pages[_currentPage]);

        // Set all the pages to off
        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(false);
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
}
