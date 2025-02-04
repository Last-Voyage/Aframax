/******************************************************************************
// File Name:       WeakPointEditor.cs
// Author:          Tommy Roberts
// Creation Date:   Feburary 4, 2025
//
// Description:     Lets player spawn weakpoints from editor
******************************************************************************/
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor class for the weakPointHandler
/// </summary>
[CustomEditor(typeof(WeakPointHandler))]
public class WeakPointEditor : Editor
{
    /// <summary>
    /// basic override for OnInspectorGUI and added a button for spawning a point
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WeakPointHandler weakPointHandler = (WeakPointHandler)target;

        if(GUILayout.Button("Spawn weak point test")){   // for some freakin reason the weak point handler script disables itself
            weakPointHandler.SpawnWeakPointEditor();     // in my test scene so I had to make this, although rather useless
        }                                           
    }
}
