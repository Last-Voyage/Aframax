/******************************************************************************
// File Name:       SavePointTrigger.cs
// Author:          Ryan Swanson
// Creation Date:   Februsary 25, 2025
//
// Description:     Saves the game on contact with a trigger
******************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Saves all important data when the player makes contact with a save point
/// </summary>
public class SavePointTrigger : MonoBehaviour
{
    public int SavePointID;

    /// <summary>
    /// Saves the game on contact
    /// </summary>
    public void PlayerContact()
    {
        SaveManager.Instance.GetGameSaveData().SetCurrentCheckPoint(SavePointID);
        SaveManager.Instance.GetGameSaveData().SetCurrentSceneIndex(SceneManager.GetActiveScene().buildIndex);
        PlayerInventory.Instance.SaveInventory();
        StoryManager.Instance.SaveData();
        SaveManager.Instance.GetOnNewCheckpoint()?.Invoke();
    }
}
