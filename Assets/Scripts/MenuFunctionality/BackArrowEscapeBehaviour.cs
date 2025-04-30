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
public class BackArrowEscapeBehaviour : MonoBehaviour
{
    private PlayerInputMap _playerInputControls;

    private Button _backArrow;

    /// <summary>
    /// Enables player input and swaps ui if needed
    /// </summary>
    private void OnEnable()
    {
        UiManager.Instance.AddToBackStack(GetComponent<Button>());
    }
}
