/*****************************************************************************
// File Name :         TitleScreenScrolling.cs
// Author :            Jeremiah Peters
// Contributor :       Nick Rice
// Creation Date :     10/27/24
//
// Brief Description : handles scrolling the title screen from top to bottom
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.DualShock;

/// <summary>
/// functionality for moving the camera on the title screen
/// </summary>
public class TitleScreenScrolling : MonoBehaviour, IUiSwap
{
    [SerializeField] private Transform _movingDestination;

    [Tooltip("how fast the screen scrolls when it is started")]
    [SerializeField] private float _screenScrollTime;

    [Tooltip("The delay before the splash effect plays")]
    [SerializeField] private float _splashEffectDelay;

    [SerializeField] 
    private TextMeshProUGUI _startGameText;
    
    [SerializeField]
    private string _controllerStartGameMessage = "A TO START";

    [SerializeField] 
    private string _keyboardStartGameMessage = "ENTER TO START";

    [SerializeField] private Animator _enterFadeOutAnimator;

    [SerializeField] private EventSystem _setUpPlayerControls;

    [SerializeField] private Animator _skullAnimator;

    [SerializeField] private float _skullTriggerPercent;

    [SerializeField] private Animator _menuAnimator;

    [SerializeField] private float _menuTriggerPercent;

    [SerializeField] private ButtonSFXManager _buttonSFXManagerReference;

    [SerializeField] private Color _titleScreenLightbarColor = Color.blue;

    private Vector3 velocity = Vector3.zero;

    private PlayerInputMap _playerInputControls;

    private bool _hasScrollingStarted = false;

    private void Awake()
    {
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.EnterTitleScreen.performed +=
            ctx => StartCoroutine(ScrollingScreen());

        _skullTriggerPercent /= 100;
        _menuTriggerPercent /= 100;
    }

    /// <summary>
    /// moves the ui up to simulate the camera moving down
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScrollingScreen()
    {
        if (!_hasScrollingStarted)
        {
            _enterFadeOutAnimator.SetTrigger("GameStarted");

            _buttonSFXManagerReference.AlwaysPlayClickSFX();

            PrimeTween.Tween.Delay(this, _splashEffectDelay, PlayMainMenuSplash);

            _hasScrollingStarted = true;

            float _screenScrollProgress;
            bool _skullTriggerSet = false;
            bool _menuTriggerSet = false;

            while (transform.position != _movingDestination.position)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _movingDestination.transform.position, ref velocity,
                    _screenScrollTime);
                yield return null;

                _screenScrollProgress = transform.position.y / _movingDestination.transform.position.y;

                if (_screenScrollProgress >= _skullTriggerPercent && _skullTriggerSet == false)
                {
                    _skullAnimator.SetTrigger("StartMoving");
                    _skullTriggerSet = true;
                }

                if (_screenScrollProgress >= _menuTriggerPercent && _menuTriggerSet == false)
                {
                    _menuAnimator.SetTrigger("StartMoving");
                    _menuTriggerSet = true;
                }

                //double checking to make sure the loop stops properly, accounting for floating point shenanigans
                if (_screenScrollProgress >= 0.99f)
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

    /// <summary>
    /// Swaps the start game UI for controller or keyboard
    /// </summary>
    public void OnUiSwap()
    {
        _startGameText.text = UiManager.IsUsingController ? _controllerStartGameMessage: _keyboardStartGameMessage;
    }

    /// <summary>
    /// Allows player input, and swaps UI if needed
    /// </summary>
    private void OnEnable()
    {
        _playerInputControls.Enable(); 
        OnUiSwap();
        
        if (!DualShockGamepad.current.IsUnityNull())
        {
            DualShockGamepad.current.SetLightBarColor(_titleScreenLightbarColor);
        }
    }

    /// <summary>
    /// Disables player controls
    /// </summary>
    private void OnDisable()
    {
        _playerInputControls.Disable();
    }
}
