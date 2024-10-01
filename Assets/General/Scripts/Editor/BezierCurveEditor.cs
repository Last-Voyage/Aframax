/*****************************************************************************
// File Name :         BezierCurveEditor.cs
// Author :            Charlie Polonus
// Creation Date :     September 23, 2024
//
// Brief Description : An custom editor class for BezierCurve that allows easy
                       editing of the bezier curve
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Streamlines the editing of bezier curves
/// </summary>
[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    // The total length of the bezier curve
    private float _bezierLength;

    /// <summary>
    /// Gets the base BezierCurve class
    /// </summary>
    private BezierCurve ActiveBezier => (BezierCurve)target;

    /// <summary>
    /// Displays the base properties as well as buttons and info to edit the bezier curve
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        GUILayout.Label("Line Length: " + _bezierLength);

        // Deletes all current points and resets the mesh
        if (GUILayout.Button("Reset Points"))
        {
            ActiveBezier.BezierPoints = new BezierPoint[0];

            // If the component is attached to a RiverSpline, reset its mesh
            if (ActiveBezier.TryGetComponent(out RiverSpline activeRiver))
            {
                activeRiver.ResetMesh();
            }
        }

        // Add a new point to the end of the bezier curve
        if (GUILayout.Button("Add Point"))
        {
            // Creates a new list if there no bezier points
            if (ActiveBezier.BezierPoints == null)
            {
                ActiveBezier.BezierPoints = new BezierPoint[0];
            }

            // Creates a new array to hold all the new points
            BezierPoint[] newPoints = new BezierPoint[ActiveBezier.BezierPoints.Length + 1];
            for (int i = 0; i < ActiveBezier.BezierPoints.Length; i++)
            {
                newPoints[i] = ActiveBezier.BezierPoints[i];
            }

            Vector2 newPoint;
            Vector2 newBackDir;
            Vector2 newForwardDir;
            if (ActiveBezier.BezierPoints.Length > 0)
            {
                // Moves the new node in front of the last one, and copies its direction
                newPoint = ActiveBezier.BezierPoints[^1].Point + ActiveBezier.BezierPoints[^1].ForwardDir.normalized * 10;
                newBackDir = ActiveBezier.BezierPoints[^1].BackDir;
                newForwardDir = ActiveBezier.BezierPoints[^1].ForwardDir;
            }
            else
            {
                // Sets the new position to an arbitrary position relative to the game object
                newPoint = (Vector2)ActiveBezier.transform.position + Vector2.up * 10;
                newBackDir = Vector2.down * 3;
                newForwardDir = Vector2.up * 3;
            }

            // Creates the new last bezier point
            newPoints[^1] = new(newPoint, newBackDir, newForwardDir);

            // Overwrites the old array with the new one
            ActiveBezier.BezierPoints = newPoints;
        }
    }

    /// <summary>
    /// Generates the bezier curve and handles to edit it
    /// </summary>
    private void OnSceneGUI()
    {
        // Edge cases
        if (ActiveBezier == null
            || ActiveBezier.BezierPoints == null
            || ActiveBezier.BezierPoints.Length == 0)
        {
            return;
        }

        _bezierLength = 0;
        BezierPoint previousPoint = ActiveBezier.BezierPoints[0];

        // Iterate through each point and draw both the handles for them and the curve they make
        for (int i = 0; i < ActiveBezier.BezierPoints.Length; i++)
        {
            // Draw the handle for the bezier
            DisplayBezierHandle(ref ActiveBezier.BezierPoints[i]);

            BezierPoint currentPoint = ActiveBezier.BezierPoints[i];

            if (previousPoint != currentPoint)
            {
                Handles.color = Color.white;

                // Draw the curve between the two bezier points
                DrawBezierLine(previousPoint, currentPoint, ActiveBezier.CurveSmoothness, out float curLength);
                _bezierLength += curLength;
            }

            previousPoint = currentPoint;
        }
    }

    /// <summary>
    /// Draw the bezier curve between two points
    /// </summary>
    /// <param name="start">The bezier to start drawing from</param>
    /// <param name="end">The bezier to draw towards</param>
    /// <param name="segmentCount">How many segments make up the curve</param>
    /// <param name="length">The total length of all the line segments</param>
    private void DrawBezierLine(BezierPoint start, BezierPoint end, int segmentCount, out float length)
    {
        // Get all the positions as well as the individual lengths of all the line segments
        Vector2[] allPositions = ActiveBezier.PointsBetweenBeziers(start, end, segmentCount, out length);

        // Iterate through each point at different linearly interpolated values of the curve, drawing a line between
        //    that point and the previous
        for (int i = 1; i < allPositions.Length; i++)
        {
            Vector3 previousPosition = new(allPositions[i - 1].x, 0, allPositions[i - 1].y);
            Vector3 currentPosition = new(allPositions[i].x, 0, allPositions[i].y);

            Handles.DrawLine(previousPosition, currentPosition);
        }
    }

    /// <summary>
    /// Draws the bezier point into the scene with tweening handles
    /// </summary>
    /// <param name="bezier">The bezier point currently being drawn</param>
    private void DisplayBezierHandle(ref BezierPoint bezier)
    {
        // Adjust the handle size so that it's the same no matter the distance from the camera
        var view = SceneView.currentDrawingSceneView;
        float handleSize = view.size / (50f / ActiveBezier.HandleSize);

        // Get the 3d representation of the current point's directions
        Vector3 backDir = new(bezier.BackDir.x, 0, bezier.BackDir.y);
        Vector3 forwardDir = new(bezier.ForwardDir.x, 0, bezier.ForwardDir.y);

        // Convert the points and directions into three 3d points
        Vector3 bezier3dPoint = new(bezier.Point.x, 0, bezier.Point.y);
        Vector3 bezier3dBack = bezier3dPoint + backDir;
        Vector3 bezier3dForward = bezier3dPoint + forwardDir;

        // Draw the middle handle
        Handles.color = Color.white;
        bezier3dPoint = Handles.FreeMoveHandle(bezier3dPoint, handleSize, Vector3.zero, Handles.DotHandleCap);

        // Draw the line to the backward direction as well as the backward handle
        Handles.color = Color.red;
        bezier3dBack = Handles.FreeMoveHandle(bezier3dPoint + backDir, handleSize, Vector3.zero, Handles.DotHandleCap);
        Handles.DrawLine(bezier3dBack, bezier3dPoint);

        // Draw the line to the forward direction as well as the forward handle
        Handles.color = Color.green;
        bezier3dForward = Handles.FreeMoveHandle(bezier3dPoint + forwardDir, handleSize, Vector3.zero, Handles.DotHandleCap);
        Handles.DrawLine(bezier3dPoint, bezier3dForward);

        // Update the bezier's variables based on what the player moved them to
        bezier.Point = new(bezier3dPoint.x, bezier3dPoint.z);
        bezier.BackDir = new Vector2(bezier3dBack.x, bezier3dBack.z) - bezier.Point;
        bezier.ForwardDir = new Vector2(bezier3dForward.x, bezier3dForward.z) - bezier.Point;
    }
}
