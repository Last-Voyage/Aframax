/*****************************************************************************
// File Name :         ProceduralVineEditor.cs
// Author :            Tommy Roberts
// Creation Date :     2/5/2025
//
// Brief Description : This script controls the procedural vine from the editor
*****************************************************************************/
using UnityEditor;
using UnityEngine;
/// <summary>
/// Custom editor class for the vine so I can mess with it without having to use the player
/// </summary>
[CustomEditor(typeof(ProceduralVine))]
public class ProceduralVineEditor : Editor
{
    /// <summary>
    /// draws the inspector with an extra test button
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ProceduralVine proceduralVine= (ProceduralVine)target;

        if(GUILayout.Button("Start Retract Test"))
        {
            proceduralVine.StartRetract();
        }

        if (GUILayout.Button("Start Appear Test"))
        {
            proceduralVine.StartAppear();
        }
    }
}