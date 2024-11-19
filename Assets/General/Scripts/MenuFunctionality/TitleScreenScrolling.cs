/*****************************************************************************
// File Name :         PauseMenu.cs
// Author :            Jeremiah Peters
// Creation Date :     10/27/24
//
// Brief Description : handles scrolling the title screen from top to bottom
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// functionality for moving the camera on the title screen
/// </summary>
public class TitleScreenScrolling : MonoBehaviour
{
    [SerializeField] private Transform _movingDestination;

    [Tooltip("how fast the screen scrolls when it is started")]
    [SerializeField] private float _screenScrollSpeed;

    [SerializeField] private Canvas _sceneCanvas;

    private PlayerInputMap _playerInputControls;

    private bool _hasScrollingStarted = false;

    private void Awake()
    {
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.EnterTitleScreen.performed += ctx => StartCoroutine(ScrollingScreen(_movingDestination.position, _screenScrollSpeed));
        if (_screenScrollSpeed == 0)
        {
            Debug.LogWarning("scroll speed is set to zero, now it won't scroll, please fix that, thanks");
        }
    }

    /// <summary>
    /// moves the ui up to simulate the camera moving down
    /// </summary>
    /// <param name="destination"></param> designated movement destination
    /// <returns></returns>
    private IEnumerator ScrollingScreen(Vector3 destination, float scrollSpeed)
    {
        if (!_hasScrollingStarted)
        {
            _hasScrollingStarted = true;
            while (transform.position != destination)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(destination.x, destination.y, transform.position.z), 
                    scrollSpeed * (_sceneCanvas.renderingDisplaySize.x/100) * Time.deltaTime);

                //for whatever reason (probably rounding bs) this coroutine doesn't actually stop itself properly when done
                //this fixes that
                if (Mathf.Approximately(transform.position.y, destination.y))
                {
                    yield break;
                }
                
                yield return null;
            }
        }
    }

    private void OnEnable()
    {
        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        _playerInputControls.Disable();
    }
}
