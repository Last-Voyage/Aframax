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
    public int CurrentCheckpoint { get; set; }
    [Tooltip("A list of strings containing the players inventory")]
    public List<string> PlayerInventory { get; set; }

    public int CurrentStoryBeat { get; set; }
    public int CurrentSceneIndex { get; set; }

    #region Audio Settings
    public float CurrentMasterVolume { get; set; }
    public float CurrentSfxVolume { get; set; }
    public float CurrentAmbienceVolume { get; set; }
    public float CurrentVoiceVolume { get; set; }
    public float CurrentMusicVolume { get; set; }
    #endregion

    #region Gameplay Settings
    public float CurrentBrightness { get; set; } = 0.5f;
    #endregion

    #region Getters

    public int GetCurrentCheckPoint() => CurrentCheckpoint;
    public List<string> GetCurrentInventory() => PlayerInventory;
    public int GetCurrentStoryBeat() => CurrentStoryBeat;
    public int GetCurrentSceneIndex() => CurrentSceneIndex;
    public float GetBrightness() => CurrentBrightness;

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

    public void SetBrightness(float newBrightness)
    {
        CurrentBrightness = newBrightness;
    }

    #endregion
}
