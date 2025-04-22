/*****************************************************************************
// File Name :         GeneralUISwapper.cs
// Author :            Nick Rice
// Creation Date :     4/19/25
//
// Brief Description : Swaps any UI a designer would need
*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Swaps any UI a designer would need (based on controller or keyboard usage)
/// </summary>
public class GeneralUISwapper : MonoBehaviour, IUiSwap
{
    [SerializeField]
    private GeneralUIData[] _allNeededUIToSwap;

    /// <summary>
    /// This swaps the Ui
    /// </summary>
    public void OnUiSwap()
    {
        foreach (var UIToSwap in _allNeededUIToSwap)
        {
            UIToSwap._uiBase.sprite = UiManager.IsUsingController ? 
                UIToSwap._controllerSpriteAsset: UIToSwap._keyboardSpriteAsset;
        }
    }

    /// <summary>
    /// This adds a listener for when the player swaps from controller to keyboard
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.GetOnSwapInput?.AddListener(OnUiSwap);
        OnUiSwap();
    }

    /// <summary>
    /// This removes the listeners to prevent memory leaks
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput?.RemoveListener(OnUiSwap);
    }
}

/// <summary>
/// The data struct used for a generalized UI swap
/// </summary>
[Serializable]
internal struct GeneralUIData
{
    [SerializeField]
    [Tooltip("The UI element that will have it's assets swapped")]
    internal Image _uiBase;

    [SerializeField]
    [Tooltip("The controller sprite asset")]
    internal Sprite _controllerSpriteAsset;
    [SerializeField]
    [Tooltip("The keyboard sprite asset")]
    internal Sprite _keyboardSpriteAsset;
}
