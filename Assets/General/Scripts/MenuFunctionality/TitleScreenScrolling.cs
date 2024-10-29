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
    [SerializeField] private Transform _bottomCameraPosition;

    [SerializeField] private float _screenScrollSpeed;

    private Camera _mainCamera;

    private PlayerInputMap _playerInputControls;

    private bool _hasScrollingStarted = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.EnterTitleScreen.performed += ctx => StartCoroutine(MoveCamera(_bottomCameraPosition.position, _screenScrollSpeed));
        if (_screenScrollSpeed == 0)
        {
            Debug.Log("scroll speed is set to zero, now it won't scroll, please fix that, thanks");
        }
    }

    /// <summary>
    /// moves the camera from the top of the screen to the bottom
    /// </summary>
    /// <param name="destination"></param> designated location of the bottom part of the screen
    /// <returns></returns>
    private IEnumerator MoveCamera(Vector3 destination, float scrollSpeed)
    {
        if (_hasScrollingStarted == false)
        {
            _hasScrollingStarted = true;
            while (_mainCamera.transform.position != destination)
            {
                _mainCamera.transform.position = Vector3.MoveTowards(_mainCamera.transform.position, new Vector3(destination.x, destination.y, -300), scrollSpeed * Time.deltaTime);

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
