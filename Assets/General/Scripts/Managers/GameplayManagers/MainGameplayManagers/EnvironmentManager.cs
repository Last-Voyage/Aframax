/******************************************************************************
// File Name:       EnvironmentManager.cs
// Author:          Ryan Swanson
// Contributor:     Nick Rice
// Creation Date:   September 15, 2024
//
// Description:     Provides the functionality behind the way the environment interacts with other elements
                    Manager to be developed as I know specifics
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides the functionality behind the way the environment interacts with other elements
/// Manager to be developed as I know specifics
/// </summary>
public class EnvironmentManager : MainGameplayManagerFramework
{
    public static EnvironmentManager Instance;

    private UnityEvent<int[]> _sendingOverChunks = new();

    private UnityEvent<GameObject[]> _allChunkObjects = new();

    private UnityEvent _changeTheChunk = new();

    /// <summary>
    /// A function that takes in the int array representing the queue, and then it 
    /// sends out an event that sends the queue over to the script that uses it 
    /// in game
    /// </summary>
    /// <param name="theQueue">The int array representing the queue of chunks</param>
    public void SendOutChunks(int[] theQueue)
    {
        GetSendingOverChunks()?.Invoke(theQueue);
    }

    #region Base Manager
    public override void SetupInstance()
    {
        base.SetupInstance();
        Instance = this;
    }
    public override void SetupMainManager()
    {
        base.SetupMainManager();
    }
    #endregion

    #region Getters
    public UnityEvent<int[]> GetSendingOverChunks() => _sendingOverChunks;

    public UnityEvent<GameObject[]> GetAllChunkObjects() => _allChunkObjects;

    public UnityEvent SendChangeTheChunk() => _changeTheChunk;
    #endregion
}
