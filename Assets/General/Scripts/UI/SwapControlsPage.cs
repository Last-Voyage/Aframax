/*****************************************************************************
// File Name :         SwapControlsPage.cs
// Author :            Jeremiah Peters
// Creation Date :     3/6/25
//
// Brief Description : swaps pages for controls script. most likely temporary.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// script will probably either be deleted or reworked to swap automatically depending on which control type is being used
/// </summary>
public class SwapControlsPage : MonoBehaviour
{
    [SerializeField] private GameObject _keyboard;
    [SerializeField] private GameObject _controller;

    [SerializeField] private Image _leftButton;
    [SerializeField] private Image _rightButton;

    private PlayerInputMap _playerInputMap;

    /// <summary>
    /// sets up input
    /// </summary>
    private void Awake()
    {
        _playerInputMap = new PlayerInputMap();
    }

    /// <summary>
    /// swaps pages depending on input value
    /// </summary>
    public void ChangePage(int value)
    {
        if (value == 1)
        {
            _keyboard.SetActive(false);
            _controller.SetActive(true);
            _leftButton.color = Color.clear;
            _rightButton.color = Color.white;
        }
        else if (value == -1)
        {
            _keyboard.SetActive(true);
            _controller.SetActive(false);
            _leftButton.color = Color.white;
            _rightButton.color = Color.clear;
        }
    }

    /// <summary>
    /// keyboard/controller support
    /// </summary>
    private void OnEnable()
    {
        _playerInputMap.Enable();
        _playerInputMap.Player.UICycling.performed += ctx => ChangePage((int)ctx.ReadValue<float>());
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        _playerInputMap.Player.UICycling.performed -= ctx => ChangePage((int)ctx.ReadValue<float>());
        _playerInputMap.Disable();
    }
}
