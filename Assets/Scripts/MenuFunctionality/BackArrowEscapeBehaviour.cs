/*****************************************************************************
// File Name :         BackArrowEscapeBehaviour.cs
// Author :            Jeremiah Peters
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
public class BackArrowEscapeBehaviour : MonoBehaviour
{
    private PlayerInputMap _playerInputControls;

    private Button _backArrow;

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

    private void OnEnable()
    {
        _playerInputControls.Enable();
    }

    private void OnDisable()
    {
        _playerInputControls.Player.UIBack.performed -= ctx => PressBackArrow();
        _playerInputControls.Disable();
    }
}
