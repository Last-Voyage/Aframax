/*****************************************************************************
// File Name :         SaveResetButton.cs
// Author :            Ryan Swanson
// Creation Date :     02/27/2025
//
// Brief Description : Resets the save data on a button
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Button to reset save data
/// </summary>
public class SaveResetButton : MonoBehaviour
{
    /// <summary>
    /// Called when the button is pressed
    /// </summary>
    public void ResetButtonPressed()
    {
        SaveManager.Instance.ResetSaveData();
        AframaxSceneManager.Instance.ReloadCurrentScene();
    }
}
