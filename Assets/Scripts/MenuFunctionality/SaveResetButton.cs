/*****************************************************************************
// File Name :         SaveResetButton.cs
// Author :            Ryan Swanson
// Creation Date :     02/27/2025
//
// Brief Description : Resets the save data on a button
*****************************************************************************/

using UnityEngine;

/// <summary>
/// Button to reset save data
/// </summary>
public class SaveResetButton : MonoBehaviour
{
    [SerializeField] private bool _doesReloadCurrentScene;
    /// <summary>
    /// Called when the button is pressed
    /// </summary>
    public void ResetButtonPressed()
    {
        SaveManager.Instance.ResetGameplaySaveData();
        if(_doesReloadCurrentScene)
        {
            AframaxSceneManager.Instance.ReloadCurrentScene();
        }
    }
}
