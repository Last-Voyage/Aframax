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
/// 

///gutuyyufyyigui
public class TitleScreenScrolling : MonoBehaviour
{
    [SerializeField] private Transform _movingDestination;

    [Tooltip("how fast the screen scrolls when it is started")]
    [SerializeField] private float _screenScrollTime;

    [Tooltip("The delay before the splash effect plays")]
    [SerializeField] private float _splashEffectDelay;

    [SerializeField] private Animator _enterFadeOutAnimator;

    [SerializeField] private EventSystem _setUpPlayerControls;

    private Vector3 velocity = Vector3.zero;

    private PlayerInputMap _playerInputControls;

    private bool _hasScrollingStarted = false;

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
            PrimeTween.Tween.Delay(this, _splashEffectDelay, PlayMainMenuSplash);

            _hasScrollingStarted = true;
            while (transform.position != _movingDestination.position)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _movingDestination.transform.position, ref velocity,
                    _screenScrollTime);
                yield return null;

                //double checking to make sure the loop stops properly, accounting for floating point shenanigans
                if (transform.position.y / _movingDestination.transform.position.y >= 0.99f)
                {
                    _setUpPlayerControls.gameObject.SetActive(true);
                    yield break;
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// Plays the sound effect of the main menu splash effect
    /// </summary>
    private void PlayMainMenuSplash()
    {
        RuntimeSfxManager.APlayOneShotSfx(FmodSfxEvents.Instance.TitleScreenSplash, Vector3.zero);
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
