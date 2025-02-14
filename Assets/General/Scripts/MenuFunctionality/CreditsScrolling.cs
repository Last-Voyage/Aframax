/*****************************************************************************
// File Name :         TitleScreenScrolling.cs
// Author :            Jeremiah Peters, Adam Garwacki
// Creation Date :     11/15/24
//
// Brief Description : Handles scrolling the credits screen from top to bottom
//                     This script was duplicated from TitleScreenScrolling.cs
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

    [SerializeField] private Canvas _sceneCanvas;

    private bool _hasScrollingStarted = false;

    private void Awake()
    {
        if (_screenScrollSpeed == 0)
        {
            Debug.LogWarning("scroll speed is set to zero, now it won't scroll, please fix that, thanks");
        }

        StartCoroutine(ScrollingScreen(_movingDestination.position, _screenScrollSpeed, _scrollWaitTime));
    }

    /// <summary>
    /// moves the ui up to simulate the camera moving down
    /// </summary>
    /// <param name="destination"></param> designated movement destination
    /// <returns></returns>
    private IEnumerator ScrollingScreen(Vector3 destination, float scrollSpeed, float scrollWaitTime)
    {
        yield return new WaitForSeconds(scrollWaitTime);

        if (!_hasScrollingStarted)
        {
            _hasScrollingStarted = true;
            while (transform.position != destination)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, new Vector3(destination.x, destination.y, transform.position.z),
                    scrollSpeed * (_sceneCanvas.renderingDisplaySize.x / 100) * Time.deltaTime);

                yield return null;
            }
        }
    }

}
