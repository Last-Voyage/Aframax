/*****************************************************************************
// Name :           SaveReconfiguration.cs
// Author :         Nick Rice
// Created :        2/24/2025
// Description :    Handles where the player spawns on scene entering
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles where the player spawns on scene entering
/// </summary>
public class SaveReconfiguration : MonoBehaviour
{
    [SerializeField] private SavePoint[] _savePoints;

    /// <summary>
    /// Loads all data in the scene
    /// </summary>
    public void LoadSave()
    {
        //Gets the player gameobject
        GameObject thePlayer = PlayerSpawnPoint.Instance.transform.GetChild(0).gameObject;

        //Gets the current save point
        SavePoint currentSavePoint = _savePoints[SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint()];


        if(currentSavePoint == null)
        {
            currentSavePoint = _savePoints[0];
            Debug.LogWarning("Couldn't find save point at ID " + 
                SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint());
        }

        if(currentSavePoint.SavePointTrigger == null)
        {
            return;
        }

        //Sets the player location to the saved location
        thePlayer.transform.position = currentSavePoint.SavePointTrigger.transform.position;

        //Enable all map chunks for this save
        foreach (GameObject mapChunks in currentSavePoint.MapChunksEnabled)
        {
            mapChunks.SetActive(true);
        }

        //Loads the player inventory
        PlayerInventory.Instance.LoadInventory();

        //Loads the current story beat
        StoryManager.Instance.LoadData();
    }
}
