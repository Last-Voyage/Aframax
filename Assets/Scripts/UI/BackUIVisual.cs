/*****************************************************************************
// File Name :         BackUIVisual.cs
// Author :            Nick Rice
// Creation Date :     4/16/25
//
// Brief Description : Allows the back UI to swap visuals
*****************************************************************************/
using UnityEngine;
using UnityEngine.UI;

public class BackUI : MonoBehaviour, IUiSwap
{
    [Tooltip("The Image on screen that will be swapped")]
    [SerializeField]
    private Image _backSprite;

    [Tooltip("The asset being put into _backSprite")]
    [SerializeField]
    private Sprite _controllerBackSpriteAsset, _keyboardBackSpriteAsset;

    /// <summary>
    /// This swaps the player ui if they are using a controller or not
    /// </summary>
    public void OnUiSwap()
    {
        _backSprite.sprite = UiManager.UsingController
            ? _controllerBackSpriteAsset
            : _keyboardBackSpriteAsset;
    }
    
    /// <summary>
    /// Swaps ui if needed and adds listeners
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.GetOnSwapInput?.AddListener(OnUiSwap);
        OnUiSwap();
    }
    
    /// <summary>
    /// Removes listeners
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput?.RemoveListener(OnUiSwap);
    }
}
