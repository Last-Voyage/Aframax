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

public class TutorialPopUp : MonoBehaviour
{
    public static TutorialPopUp ActiveTutorial = null;

    [Header("References")]
    [SerializeField] private Canvas _ui;
    [SerializeField] private GameObject[] _pages;
    [SerializeField] private TMP_Text _leftArrow;
    [SerializeField] private TMP_Text _rightArrow;
    private int _currentPage;
    private bool _hasRead;

    private void Start()
    {
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(15);

        OpenTutorialPopUp();
    }

    public void OpenTutorialPopUp()
    {
        _ui.enabled = true;

        // Free the mouse and freeze the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        // Reset the page counter to the first page and activate the note
        _currentPage = 0;
        ActiveTutorial = this;
        ChangePage(_currentPage);
    }

    public void ChangePage(int value)
    {
        StopPage(_pages[_currentPage]);

        // Clamp the page to the bounds of the note, then assign the text
        _currentPage = Mathf.Clamp(_currentPage + value, 0, _pages.Length - 1);

        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(i == _currentPage);
        }

        StartPage(_pages[_currentPage]);

        _leftArrow.color = _currentPage == 0 ? Color.clear : Color.white;
        _rightArrow.color = _currentPage == _pages.Length - 1 ? Color.clear : Color.white;
    }

    private void StopPage(GameObject page)
    {
        VideoPlayer pageVideo = page.GetComponentInChildren<VideoPlayer>();

        if (pageVideo.clip != null)
        {
            pageVideo.time = 0;
            pageVideo.Stop();
        }
    }

    private void StartPage(GameObject page)
    {
        VideoPlayer pageVideo = page.GetComponentInChildren<VideoPlayer>();
        RawImage pageVideoImage = page.GetComponentInChildren<RawImage>();
        Image pageImage = page.GetComponentInChildren<Image>();

        if (pageVideo.clip != null)
        {
            pageVideo.targetCamera = Camera.current;
            pageVideo.time = 0;
            pageVideo.Play();
        }
    }

    private void CloseTutorialPopUp()
    {
        ActiveTutorial = null;
        _hasRead = true;

        StopPage(_pages[_currentPage]);

        for (int i = 0; i < _pages.Length; i++)
        {
            _pages[i].SetActive(false);
        }

        _ui.enabled = false;

        // Free the mouse and freeze the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    public static void ExitActivePopUp()
    {
        ActiveTutorial.CloseTutorialPopUp();
    }
}
