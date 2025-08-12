/*****************************************************************************
// File Name :         CreditsSkipUI.cs
// Author :            Ryan Swanson
// Creation Date :     8/7/2025
//
// Brief Description : Handles the skip
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality for skipping credits and cinematics
/// </summary>
public class CreditsCinematicsSkipUI : MonoBehaviour
{
    [SerializeField] private Animator _skipPromptTextAnimator;

    [SerializeField] private float _skipPromptDuration;

    [SerializeField] private UnityEvent _onSkip;
    

    private PlayerInputMap _playerInputControls;

    private bool _skipTextActive;
    
    private const string _SKIP_BUTTON_ACTIVE = "VisualsActive";
    private int _skip_Button_Active_Hash = Animator.StringToHash(_SKIP_BUTTON_ACTIVE);

    /// <summary>
    /// Performs set up of controls when activated
    /// </summary>
    private void Start()
    {
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Enable();

        _playerInputControls.Player.SkipPrompt.started += SkipPromptPressed;
        _playerInputControls.Player.SkipCinematic.started += SkipButtonPressed;
    }

    /// <summary>
    /// Performs clean up of controls when destroyed
    /// </summary>
    private void OnDestroy()
    {
        _playerInputControls.Player.SkipPrompt.started -= SkipPromptPressed;
        _playerInputControls.Player.SkipCinematic.started -= SkipButtonPressed;
        _playerInputControls.Disable();
    }

    /// <summary>
    /// Starts the process of showing the skip text
    /// </summary>
    /// <param name="ctx"></param>
    private void SkipPromptPressed(InputAction.CallbackContext ctx)
    {
        if (!_skipTextActive)
        {
            StartCoroutine(ShowSkipText());
        }
    }
    
    /// <summary>
    /// Skips the cinematic
    /// </summary>
    /// <param name="ctx"> The input context </param>
    private void SkipButtonPressed(InputAction.CallbackContext ctx)
    {
        if (_skipTextActive)
        {
            InvokeOnSkip();
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

    /// <summary>
    /// Invokes the event for skipping credits/cinematics
    /// </summary>
    private void InvokeOnSkip()
    {
        _onSkip.Invoke();
    }
}
