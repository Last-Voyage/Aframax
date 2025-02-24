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
    public bool TempSaveBool;
    [Tooltip("Number keeping track of the player checkpoint")]
    public int CurrentCheckpoint;
    [Tooltip("A list of strings containing the players inventory")]
    public List<string> PlayerInventory;

    #region Getters

    public bool GetTempBool() => TempSaveBool;
    public int GetCurrentCheckPoint() => CurrentCheckpoint;
    public List<string> GetCurrentInventory() => PlayerInventory;

    #endregion

    #region Setters

    public void SetTempBool(bool newTemp)
    {
        TempSaveBool = newTemp;
    }

    public void SetCurrentCheckPoint(int newCheckpoint)
    {
        CurrentCheckpoint = newCheckpoint;
    }

    public void SetPlayerInventory(List<string> newInventory)
    {
        PlayerInventory = newInventory;
    }

    #endregion
}
