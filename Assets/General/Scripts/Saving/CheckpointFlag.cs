/******************************************************************************
// File Name:       CheckpointFlag.cs
// Author:          Nick Rice
// Creation Date:   February 22, 2025
//
// Description:     This script sends an event for any new checkpoint that has been reached
******************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script sends an event when a new checkpoint is reached
/// </summary>
public class CheckpointFlag : MonoBehaviour
{

    /// <summary>
    /// The different spawns the player can have
    /// </summary>
    private enum TheCheckpoints
    {
        TopOfBoat,
        Maze1,
        Maze2,
        Maze3
    }
    
    [SerializeField] 
    private TheCheckpoints _whichCheckpoint;

    /// <summary>
    /// When this object is enabled, it will try to change the player's current check point
    /// It will send out an event when it does
    /// </summary>
    private void OnEnable()
    {
        if (SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint() >= (int)_whichCheckpoint)
        {
            return;
        }
        
        StartCoroutine(StartSavingFunctionality());
    }

    /// <summary>
    /// This function will save the players checkpoint and build index
    /// And it will invoke an event to make other scripts save their data
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartSavingFunctionality()
    {
        yield return new WaitForSeconds(.5f);
        SaveManager.Instance.GetGameSaveData().SetCurrentCheckPoint((int)_whichCheckpoint);
        SaveManager.Instance.GetGameSaveData().SetCurrentSceneIndex(SceneManager.GetActiveScene().buildIndex);
        SaveManager.Instance.GetOnNewCheckpoint()?.Invoke();
    }
}
