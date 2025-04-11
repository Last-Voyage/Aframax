/*****************************************************************************
// Name :           SaveReconfiguration.cs
// Author :         Nick Rice
// Contributors:    Ryan Swanson
// Created :        2/24/2025
// Description :    Handles where the player spawns on scene entering
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles where the player spawns on scene entering
/// </summary>
public class SaveReconfiguration : MonoBehaviour
{
    public static SaveReconfiguration Instance;

    public SavePoint[] SavePoints;

    public void StartLoadSave(MazeSubSceneManager mazeSubSceneManager)
    {
        StartCoroutine(LoadSave(mazeSubSceneManager));
    }

    /// <summary>
    /// Loads all data in the scene
    /// </summary>
    private IEnumerator LoadSave(MazeSubSceneManager mazeSubSceneManager)
    {
        Instance = this;

        //Gets the player gameobject
        GameObject thePlayer = PlayerSpawnPoint.Instance.transform.GetChild(0).gameObject;

        //Gets the current save point
        SavePoint currentSavePoint = SavePoints[SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint()];

        if(currentSavePoint == null)
        {
            currentSavePoint = SavePoints[0];
            Debug.LogWarning("Couldn't find save point at ID " + 
                SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint());
        }
        
        //Enable all map chunks for this save
        foreach (int mapChunks in currentSavePoint.MapChunksEnabled)
        {
            mazeSubSceneManager.PreLoadMazeScene(mapChunks);
            mazeSubSceneManager.LoadMazeAdditive(mapChunks);
            mazeSubSceneManager.PreLoadMazeScene(mapChunks + 1);
        }

        while(currentSavePoint.SavePointTrigger == null)
        {
            yield return null;
        }

        SavePointTrigger[] tempSavePoints = FindObjectsOfType<SavePointTrigger>();

        foreach (SavePointTrigger t in tempSavePoints)
        {
            t.Activate();
        }
        
        //Sets the player location to the saved location
        thePlayer.transform.position = currentSavePoint.SavePointTrigger.transform.position;

        currentSavePoint.SavePointTrigger.EnableOnLoadObjects();

        //Loads the player inventory
        PlayerInventory.Instance.LoadInventory();

        //Loads the current story beat
        StoryManager.Instance.LoadData();
    }
}
