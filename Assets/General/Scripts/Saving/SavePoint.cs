/******************************************************************************
// File Name:       SavePoint.cs
// Author:          Ryan Swanson
// Creation Date:   Februsary 25, 2025
//
// Description:     Holds the data associated with a segment of the game
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the data associated with each segment of the game we are saving
/// </summary>
[System.Serializable]
public class SavePoint
{
    public SavePointTrigger SavePointTrigger;
    public List<GameObject> MapChunksEnabled;
}
