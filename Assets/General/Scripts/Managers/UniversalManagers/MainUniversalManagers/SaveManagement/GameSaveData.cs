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

/// <summary>
/// Holds the data which is being saved
/// Save data is read from text file and stored into Game Save Data
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public bool TempSaveBool;

    #region Getters

    public bool GetTempBool() => TempSaveBool;

    #endregion

    #region Setters

    public void SetTempBool(bool newTemp)
    {
        TempSaveBool = newTemp;
    }

    #endregion
}
