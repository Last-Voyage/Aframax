/*****************************************************************************
// File Name :         SwapControlsPage.cs
// Author :            Jeremiah Peters
// Contributors:       Nick Rice
// Creation Date :     3/6/25
//
// Brief Description : Swaps pages for controls script
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// This swaps the controls page depending on which control type is being used
/// </summary>
public class SwapControlsPage : MonoBehaviour, IUiSwap
{
    [SerializeField] private GameObject _keyboard;
    [SerializeField] private GameObject _controller;
    
    public void OnUiSwap()
    {
        _controller.SetActive(UiManager.IsUsingController);
        _keyboard.SetActive(!UiManager.IsUsingController);
    }

    /// <summary>
    /// keyboard/controller support
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.GetOnSwapInput?.AddListener(OnUiSwap);
        OnUiSwap();
    }

    /// <summary>
    /// Prevents memory leaks
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput?.RemoveListener(OnUiSwap);
    }
}
