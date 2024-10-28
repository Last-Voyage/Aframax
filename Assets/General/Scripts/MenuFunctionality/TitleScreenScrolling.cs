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

public class TitleScreenScrolling : MonoBehaviour
{
    [SerializeField] private Vector3 bottomCameraPosition;

    [SerializeField] private float screenScrollSpeed;

    private Camera mainCamera;

    private PlayerInputMap _playerInputControls;

    private bool _scrollingStarted = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.EnterTitleScreen.performed += ctx => StartCoroutine(MoveCamera(bottomCameraPosition, screenScrollSpeed));
    }

    /// <summary>
    /// moves the camera from the top of the screen to the bottom
    /// </summary>
    /// <param name="destination"></param> designated location of the bottom part of the screen
    /// <returns></returns>
    private IEnumerator MoveCamera(Vector3 destination, float scrollSpeed)
    {
        if (_scrollingStarted == false)
        {
            _scrollingStarted = true;
            while (mainCamera.transform.position != destination)
            {
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, destination, scrollSpeed * Time.deltaTime);

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
