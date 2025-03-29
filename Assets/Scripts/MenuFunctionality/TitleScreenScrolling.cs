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
using UnityEngine.EventSystems;

/// <summary>
/// functionality for moving the camera on the title screen
/// </summary>
public class TitleScreenScrolling : MonoBehaviour
{
    [SerializeField] private Animator _enterFadeOutAnimator;

    [SerializeField] private Animator _titleScrollAnimator;

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
    /// <param name="destination"> The destination to move the title screen to </param>
    /// <param name="scrollSpeed"> The speed to move the title screen at </param>
    /// <returns></returns>
    private IEnumerator ScrollingScreen()
    {
        _enterFadeOutAnimator.SetTrigger("GameStarted");
        _titleScrollAnimator.SetTrigger("EnterPressed");

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
