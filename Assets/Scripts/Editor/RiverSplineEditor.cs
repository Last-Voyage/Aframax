/*****************************************************************************
// File Name :         RiverSplineEditor.cs
// Author :            Charlie Polonus
// Creation Date :     September 24, 2024
//
// Brief Description : An custom editor class for RiverSpline that creates an
                       easy in-editor way to generate river meshes
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Streamlines the making of river meshes
/// </summary>
[CustomEditor(typeof(RiverSpline))]
public class RiverSplineEditor : Editor
{
    /// <summary>
    /// Gets the base RiverSpline class
    /// </summary>
    private RiverSpline RiverSpline => (RiverSpline)target;

    /// <summary>
    /// Displays the base properties as well as buttons to generate the mesh
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Mesh"))
        {
            RiverSpline.DrawMesh();
        }
    }
}
