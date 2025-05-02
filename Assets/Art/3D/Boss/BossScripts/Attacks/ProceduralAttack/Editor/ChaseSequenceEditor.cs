/*****************************************************************************
// File Name :         ChaseSequenceEditor.cs
// Author :            Tommy Roberts
// Creation Date :     2/19/2025
//
// Brief Description : This scirpt helps you test the chase sequence in editor
*****************************************************************************/
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor class vine chase group so I can activate it without player triggering
/// </summary>
[CustomEditor(typeof(ChaseVineGroup))]
public class ChaseSequenceEditor : Editor
{
    /// <summary>
    /// draws the inspector with an extra test button
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChaseVineGroup chaseVineGroup= (ChaseVineGroup)target;
        if(GUILayout.Button("Start Chase Test"))
        {
            chaseVineGroup.ActivateThisGroupOfVines();
        }
    }
}
