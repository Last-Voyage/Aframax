/*****************************************************************************
// File Name :         TitleScreenScrolling.cs
// Author :            Jeremiah Peters
// Creation Date :     10/27/24
//
// Brief Description : handles scrolling the title screen from top to bottom
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// functionality for moving the camera on the title screen
/// </summary>
public class TitleScreenScrolling : MonoBehaviour
{
    [SerializeField] private Animator _enterFadeOutAnimator;

    [SerializeField] private Canvas _sceneCanvas;

    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 0.3f;

    [SerializeField] private EventSystem _setUpPlayerControls;

    [SerializeField] private Transform _movingDestination;

    [Tooltip("how fast the screen scrolls when it is started")]
    [SerializeField] private float _screenScrollTime;

    private bool _hasScrollingStarted = false;

    private PlayerInputMap _playerInputControls;

    private void Awake()
    {
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.EnterTitleScreen.performed +=
            ctx => StartCoroutine(ScrollingScreen());
    }

    /// <summary>
    /// moves the ui up to simulate the camera moving down
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollingScreen()
    {
        _enterFadeOutAnimator.SetTrigger("GameStarted");

        if (!_hasScrollingStarted)
        {
            _hasScrollingStarted = true;

            while (transform.position != _movingDestination.position)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _movingDestination.transform.position, ref velocity,
                    _screenScrollTime * Time.deltaTime * 2000/_sceneCanvas.renderingDisplaySize.y);
                yield return null;

                //double checking to make sure the loop stops properly, accounting for floating point shenanigans
                if (transform.position.y / _movingDestination.transform.position.y >= 0.99f)
                {
                    _setUpPlayerControls.gameObject.SetActive(true);
                    yield break;
                }
            }
        }
        yield return null;
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
