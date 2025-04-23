/******************************************************************************
// File Name:       SavePointTrigger.cs
// Author:          Ryan Swanson
// Creation Date:   February 25, 2025
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

    [SerializeField] private GameObject[] _objectsToEnableOnLoad;

    private bool _savePointActivated = false;

    /// <summary>
    /// Activates the save point if it hasn't already
    /// </summary>
    private void Start()
    {
        if(!_savePointActivated)
        {
            Activate();
        }
    }

    /// <summary>
    /// Tells the save point trigger to add itself to the save reconfiguration
    /// </summary>
    public void Activate()
    {
        _savePointActivated = true;
        SaveReconfiguration.Instance.SavePoints[SavePointID].SavePointTrigger = this;
    }

    /// <summary>
    /// Sets active all objects that should appear when loading from this save point
    /// </summary>
    public void EnableOnLoadObjects()
    {
        foreach(GameObject obj in _objectsToEnableOnLoad)
        {
            obj.SetActive(true);
        }
    }

    /// <summary>
    /// Saves the game on contact
    /// </summary>
    public void PlayerContact()
    {
        SaveManager.Instance.SavedGameplayData();
        SaveManager.Instance.GetGameSaveData().SetCurrentCheckPoint(SavePointID);
        SaveManager.Instance.GetGameSaveData().SetCurrentSceneIndex(SceneManager.GetActiveScene().buildIndex);
        PlayerInventory.Instance.SaveInventory();
        StoryManager.Instance.SaveData();
        SaveManager.Instance.GetOnNewCheckpoint()?.Invoke();
    }
}
