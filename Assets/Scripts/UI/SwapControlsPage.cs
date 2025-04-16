/*****************************************************************************
// File Name :         SwapControlsPage.cs
// Author :            Jeremiah Peters
// Creation Date :     3/6/25
//
// Brief Description : swaps pages for controls script. most likely temporary. (lol)
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// script will probably either be deleted or reworked to swap automatically depending on which control type is being used
/// </summary>
public class SwapControlsPage : MonoBehaviour, IUiSwap
{
    [SerializeField] private GameObject _keyboard;
    [SerializeField] private GameObject _controller;

    //[SerializeField] private Image _leftButton;
    //[SerializeField] private Image _rightButton;

    //[Header("Back button")]
    //[SerializeField] private Image _backButton;
    //[SerializeField] private Image _controllerBackButtonAsset, _keyboardBackButtonAsset;
    
    

    // We won't need this. It's supposed to automatically swap to the right contorller input
    /*[Header("Left page button")]
    [SerializeField] private Image _leftPageButton;
    [SerializeField] private Image _controllerLeftPageButtonAsset, _keyboardLeftPageButtonAsset;

    [Header("Right page button")]
    [SerializeField] private Image _rightPageButton;
    [SerializeField] private Image _controllerRightPageButtonAsset, _keyboardRightPageButtonAsset;*/

    //private PlayerInputMap _playerInputMap;

    /// <summary>
    /// sets up input
    /// </summary>
    /*private void Awake()
    {
        _playerInputMap = new PlayerInputMap();
    }*/

    /// <summary>
    /// swaps pages depending on input value
    /// </summary>
    /*public void ChangePage(int value)
    {
        if (value == -1)
        {
            _keyboard.SetActive(false);
            _controller.SetActive(true);
            //_leftButton.color = Color.clear;
            //_rightButton.color = Color.white;
        }
        else if (value == 1)
        {
            _keyboard.SetActive(true);
            _controller.SetActive(false);
            //_leftButton.color = Color.white;
            //_rightButton.color = Color.clear;
        }
    }*/

    /// <summary>
    /// This swaps the button UI depending on if
    /// </summary>
    public void OnUiSwap()
    {
        if (UiManager.UsingController)
        {
            //_backButton = _controllerBackButtonAsset;
            _controller.SetActive(true);
            _keyboard.SetActive(false);
            //_leftPageButton = _controllerLeftPageButtonAsset;
            //_rightPageButton = _controllerRightPageButtonAsset;
        }
        else
        {
            //_backButton = _keyboardBackButtonAsset;
            _controller.SetActive(false);
            _keyboard.SetActive(true);
            //_leftPageButton = _keyboardLeftPageButtonAsset;
            //_rightPageButton = _keyboardRightPageButtonAsset;
        }
    }

    /// <summary>
    /// keyboard/controller support
    /// </summary>
    private void OnEnable()
    {
        //_playerInputMap.Enable();
        //_playerInputMap.Player.UICycling.performed += ctx => ChangePage((int)ctx.ReadValue<float>());
        
        UiManager.Instance.GetOnSwapInput?.AddListener(OnUiSwap);
        OnUiSwap();
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput?.RemoveListener(OnUiSwap);
        //_playerInputMap.Player.UICycling.performed -= ctx => ChangePage((int)ctx.ReadValue<float>());
        //_playerInputMap.Disable();
    }
}
