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
using TMPro;
using UnityEngine.InputSystem;
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
    
    [Space]
    [SerializeField] private Animator _skipPromptTextAnimator;

    [SerializeField] private float _skipPromptDuration;

    private PlayerInputMap _playerInputControls;

    private bool _skipTextActive;
    
    private const string _SKIP_BUTTON_ACTIVE = "VisualsActive";
    private int _skip_Button_Active_Hash = Animator.StringToHash(_SKIP_BUTTON_ACTIVE);
    
    /// <summary>
    /// Performs all functionality needed when this object is created
    /// </summary>
    private void Awake()
    {
        if (_screenScrollSpeed == 0)
        {
            Debug.LogWarning("scroll speed is set to zero, now it won't scroll, please fix that, thanks");
        }
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Enable();
        
        _playerInputControls.Player.SkipPrompt.started += SkipButtonPressed;

        StartCoroutine(ScrollingScreen(_movingDestination.position, _screenScrollSpeed, _scrollWaitTime));
    }

    /// <summary>
    /// Called to perform any needed clean up relating to the credits
    /// </summary>
    void OnDestroy()
    {
        _playerInputControls.Player.SkipPrompt.started -= SkipButtonPressed;
        
        _playerInputControls.Disable();
    }

    #region Skip
    /// <summary>
    /// Starts the process of showing the skip text
    /// </summary>
    /// <param name="ctx"> The input context </param>
    private void SkipButtonPressed(InputAction.CallbackContext ctx)
    {
        if (_skipTextActive)
        {
            ReturnToMainMenu();
        }
        else
        {
            StartCoroutine(ShowSkipText());
        }
    }

    /// <summary>
    /// makes the skip text appear on screen, then disappear after a while
    /// also enables and disables the input for skipping
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowSkipText()
    {
        _skipTextActive = true;
        float currentTime = 0;
        _skipPromptTextAnimator.SetBool(_skip_Button_Active_Hash,_skipTextActive);

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime / _skipPromptDuration;
            yield return null;
        }    
        
        _skipTextActive = false;
        _skipPromptTextAnimator.SetBool(_skip_Button_Active_Hash,_skipTextActive);
    }

    #endregion

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
                    transform.position, new Vector3(transform.position.x, destination.y, transform.position.z),
                    scrollSpeed * Time.deltaTime);

                yield return null;
            }
        }

        // Once credits are over, loads the main menu
        if (SteamManager.Initialized)
        {
            SteamAchievements.Instance.CompleteCredits();
        }
        ReturnToMainMenu();
    }

    /// <summary>
    /// Returns the player to the main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        AframaxSceneManager.Instance.StartAsyncSceneLoadViaID(AframaxSceneManager.Instance.MainMenuSceneIndex, 0);
    }
}
