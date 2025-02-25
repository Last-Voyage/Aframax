/*****************************************************************************
// Name :           SaveReconfiguration.cs
// Author :         Nick Rice
// Created :        2/24/2025
// Description :    Handles where the player spawns on scene entering
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveReconfiguration : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _spawnLocations;

    [SerializeField] 
    private List<GameObject> _mapChunks;

    // Not an elegant solution, but one that works
    [SerializeField] 
    private GameObject _thePlayer;
    
    /// <summary>
    /// When this object is enabled, it will move the player based on their checkpoint
    /// If at all
    /// </summary>
    private void OnEnable()
    {
        switch (SaveManager.Instance.GetGameSaveData().GetCurrentCheckPoint())
        {
            case 2:
                _mapChunks[1].SetActive(true);
                _mapChunks[0].SetActive(false);
                _thePlayer.transform.position = _spawnLocations[1].position;
                break;
            case 3:
                _mapChunks[2].SetActive(true);
                _mapChunks[0].SetActive(false);
                _thePlayer.transform.position = _spawnLocations[2].position;
                break;
                default:
                    Debug.Log("There should be no changes in the spawn");
                break;
        }
    }
}
