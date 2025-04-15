/*****************************************************************************
// File Name :         PauseMenuRefresher.cs
// Author :            Adam Garwacki
// Creation Date :     4/14/25
//
// Brief Description : Makes the pause menu's state return to default when it
//                     is closed.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns the pause menu to its default appearance when it is closed.
/// </summary>
public class PauseMenuRefresher : MonoBehaviour
{
    [SerializeField] private GameObject _basePauseUI;
    [SerializeField] private GameObject _pauseUIPage2;

    private void OnDisable()
    {
        // Ensures pause menu is in default state
        _pauseUIPage2.SetActive(false);
        _basePauseUI.SetActive(true);
    }
}
