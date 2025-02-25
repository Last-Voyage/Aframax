/******************************************************************************
// File Name:       GameSaveData.cs
// Author:          Ryan Swanson
// Creation Date:   November 19th, 2024
//
// Description:     Contains the data being saved by the SaveManager
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Holds the data which is being saved
/// Save data is read from text file and stored into Game Save Data
/// </summary>
[System.Serializable]
public class GameSaveData
{
    [Tooltip("Number keeping track of the player checkpoint")]
    public int CurrentCheckpoint;
    [Tooltip("A list of strings containing the players inventory")]
    public List<string> PlayerInventory;

    public int CurrentStoryBeat;
    public int CurrentSceneIndex;

    #region Getters
    
    public int GetCurrentCheckPoint() => CurrentCheckpoint;
    public List<string> GetCurrentInventory() => PlayerInventory;
    public int GetCurrentStoryBeat() => CurrentStoryBeat;
    public int GetCurrentSceneIndex() => CurrentSceneIndex;

    #endregion

    #region Setters

    public void SetCurrentCheckPoint(int newCheckpoint)
    {
        CurrentCheckpoint = newCheckpoint;
    }

    public void SetPlayerInventory(List<string> newInventory)
    {
        PlayerInventory = newInventory;
    }

    public void SetCurrentStoryBeat(int newStoryBeat)
    {
        CurrentStoryBeat = newStoryBeat;
    }

    public void SetCurrentSceneIndex(int newSceneIndex)
    {
        CurrentSceneIndex = newSceneIndex;
    }

    #endregion
}
