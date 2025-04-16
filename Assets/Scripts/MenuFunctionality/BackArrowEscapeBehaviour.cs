/*****************************************************************************
// File Name :         BackArrowEscapeBehaviour.cs
// Author :            Jeremiah Peters
// Contributor:        Nick Rice
// Creation Date :     11/18/24
//
// Brief Description : attached to the back arrow inside submenus, adds the functionality to press escape to go back
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// allows pressing escape to push a button that this script is attached to
/// </summary>
public class BackArrowEscapeBehaviour : MonoBehaviour, IUiSwap
{
    private PlayerInputMap _playerInputControls;

    private Button _backArrow;

    [SerializeField]
    private Sprite _backArrowSprite;

    [SerializeField]
    private Sprite _controllerBackArrowSpriteAsset, _keyboardBackArrowSpriteAsset;

    private void Awake()
    {
        //initialize input
        _playerInputControls = new PlayerInputMap();
        _playerInputControls.Player.UIBack.performed += ctx => PressBackArrow();

        _backArrow = GetComponent<Button>();
    }

    /// <summary>
    /// presses the back arrow that this is attached to
    /// </summary>
    private void PressBackArrow()
    {
        _backArrow.onClick.Invoke();
    }

    /// <summary>
    /// This swaps the player ui if they are using a controller or not
    /// </summary>
    public void OnUiSwap()
    {
        _backArrowSprite = UiManager.UsingController
            ? _controllerBackArrowSpriteAsset
            : _keyboardBackArrowSpriteAsset;
    }

    /// <summary>
    /// Enables player input and swaps ui if needed
    /// </summary>
    private void OnEnable()
    {
        _playerInputControls.Enable();
        UiManager.Instance.GetOnSwapInput?.AddListener(OnUiSwap);
        OnUiSwap();
    }

    /// <summary>
    /// Disables player input and removes listeners
    /// </summary>
    private void OnDisable()
    {
        UiManager.Instance.GetOnSwapInput?.RemoveListener(OnUiSwap);
        _playerInputControls.Player.UIBack.performed -= ctx => PressBackArrow();
        _playerInputControls.Disable();
    }
}
