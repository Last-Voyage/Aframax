/*****************************************************************************
// File Name :         CreditsScrolling.cs
// Author :            Adam Garwacki, Jeremiah Peters
// Creation Date :     11/15/24
//
// Brief Description : Handles scrolling the credits screen from top to bottom
//                     This script was adapted from TitleScreenScrolling.cs
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// functionality for moving the camera on the title screen
/// </summary>
public class CreditsScrolling : MonoBehaviour
{
    [SerializeField] private Transform _movingDestination;

    [SerializeField] private float _screenScrollSpeed;
    [SerializeField] private float _scrollWaitTime;
    [SerializeField] private float _scrollEndWaitTime;

    [SerializeField] private Canvas _sceneCanvas;

    private bool _hasScrollingStarted = false;

    /// <summary>
    /// Performs needed set up
    /// </summary>
    private void Awake()
    {
        if (_screenScrollSpeed == 0)
        {
            Debug.LogWarning("scroll speed is set to zero, now it won't scroll, please fix that, thanks");
        }

        StartScreenScroll();
    }

    /// <summary>
    /// Starts the coroutine for scrolling the screen
    /// </summary>
    private void StartScreenScroll()
    {
        if (!_hasScrollingStarted)
        {
            _hasScrollingStarted = true;
            StartCoroutine(ScrollingScreen(_movingDestination.position));
        }
    }

    /// <summary>
    /// moves the ui up to simulate the camera moving down
    /// </summary>
    /// <param name="destination"> designated movement destination</param>
    /// <returns></returns>
    private IEnumerator ScrollingScreen(Vector3 destination)
    {
        yield return new WaitForSeconds(_scrollWaitTime);
        
        while (transform.position.y < destination.y)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, new Vector3(transform.position.x, destination.y, transform.position.z),
                _screenScrollSpeed * Time.deltaTime);

            yield return null;
            
        }
        yield return new WaitForSeconds(_scrollEndWaitTime);

        // Once credits are over, loads the main menu
        if (SteamManager.Initialized)
        {
            SteamAchievements.Instance.CompleteCredits();
        }
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(AframaxSceneManager.Instance.MainMenuSceneIndex, 0);
    }

}
