/*****************************************************************************
// File Name :         BezierCurveEditor.cs
// Author :            Charlie Polonus
// Creation Date :     September 23, 2024
//
// Brief Description : An custom editor class for BezierCurve that allows easy
                       editing of the Bézier curve
*****************************************************************************/

using UnityEngine;
using UnityEditor;

/// <summary>
/// Streamlines the editing of Bézier curves
/// </summary>
[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    // The total length of the Bézier curve
    private float _bezierLength;

    // Whether the inspector is in debug mode
    private bool _isInDebugMode;

    // The ideal y value
    private float _yValue;

    /// <summary>
    /// Gets the base BezierCurve class
    /// </summary>
    private BezierCurve ActiveBezier => (BezierCurve)target;
    private Transform ActiveTransform => ((BezierCurve)target).GetComponent<Transform>();

    /// <summary>
    /// Displays the base properties as well as buttons and info to edit the Bézier curve
    /// </summary>
    public override void OnInspectorGUI()
    {
        _isInDebugMode = EditorGUILayout.Toggle("Debug Mode", _isInDebugMode);
        ActiveBezier.Tweening = EditorGUILayout.Toggle("Tweening Mode", ActiveBezier.Tweening);
        GUILayout.Space(10);

        // Easier layout if not in debug mode
        if (_isInDebugMode)
        {
            base.OnInspectorGUI();
        }
        else
        {
            // Display general editor settings
            GUILayout.Label("Editor Settings", EditorStyles.boldLabel);
            ActiveBezier.HandleSize = 
                Mathf.Max(EditorGUILayout.FloatField("Handle Size", ActiveBezier.HandleSize), 0);
            
            GUILayout.Space(10);
            
            // Display curve settings
            GUILayout.Label("Curve Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            ActiveBezier.CurveIntensity = EditorGUILayout.FloatField("Intensity", ActiveBezier.CurveIntensity);
            ActiveBezier.CurveSmoothness = 
                Mathf.Max(EditorGUILayout.IntField("Smoothness", ActiveBezier.CurveSmoothness), 1);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        if (ActiveBezier.BezierPoints != null)
        {
            // Display curve info
            GUILayout.Label("Bezier Points: " + ActiveBezier.BezierPoints.Length);
            GUILayout.Label("Line Segments: " + Mathf.Max((ActiveBezier.BezierPoints.Length - 1) * 
                                                          ActiveBezier.CurveSmoothness, 0));
            GUILayout.Label("Line Length: " + _bezierLength);
        }

        GUILayout.BeginHorizontal();

        // Add a new point to the end of the Bézier curve
        if (GUILayout.Button("Add Point"))
        {
            // Creates a new list if there are no bezier points
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

            Vector3 newPoint;
            Vector3 newBackDir;
            Vector3 newForwardDir;
            if (ActiveBezier.BezierPoints.Length > 0)
            {
                // Moves the new node in front of the last one, and copies its direction
                newPoint = 
                    ActiveBezier.BezierPoints[^1].Point + ActiveBezier.BezierPoints[^1].ForwardDir.normalized * 5;
                newBackDir = ActiveBezier.BezierPoints[^1].BackDir.normalized * 3;
                newForwardDir = ActiveBezier.BezierPoints[^1].ForwardDir.normalized * 3;
            }
            else
            {
                // Sets the new position to an arbitrary position relative to the game object
                newPoint = Vector3.forward * 3;
                newBackDir = Vector3.back * 3;
                newForwardDir = Vector3.forward * 3;
            }

            // Creates the new last bezier point
            newPoints[^1] = new(newPoint, newBackDir, newForwardDir);

            // Overwrites the old array with the new one
            ActiveBezier.BezierPoints = newPoints;
        }

        // Delete the last point of the curve
        if (GUILayout.Button("Remove Last Point"))
        {
            // Creates a new array to hold all the new points
            BezierPoint[] newPoints = new BezierPoint[Mathf.Max(ActiveBezier.BezierPoints.Length - 1, 0)];
            for (int i = 0; i < ActiveBezier.BezierPoints.Length - 1; i++)
            {
                newPoints[i] = ActiveBezier.BezierPoints[i];
            }

            // Overwrites the old array with the new one
            ActiveBezier.BezierPoints = newPoints;
        }

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

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        _yValue = EditorGUILayout.FloatField("Y Value", _yValue);
        if (GUILayout.Button("Flatten to Y Value"))
        {
            // Edits all points to be at a set y value
            for (int i = 0; i < ActiveBezier.BezierPoints.Length; i++)
            {
                ActiveBezier.BezierPoints[i].Point =
                    new(ActiveBezier.BezierPoints[i].Point.x, _yValue, ActiveBezier.BezierPoints[i].Point.z);
                ActiveBezier.BezierPoints[i].BackDir = 
                    new(ActiveBezier.BezierPoints[i].BackDir.x, 0, ActiveBezier.BezierPoints[i].BackDir.z);
                ActiveBezier.BezierPoints[i].ForwardDir = 
                    new(ActiveBezier.BezierPoints[i].ForwardDir.x, 0, ActiveBezier.BezierPoints[i].ForwardDir.z);
            }
        }

        SceneView.RepaintAll();
    }

    /// <summary>
    /// Generates the Bézier curve and handles to edit it
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
            // Draw the handle for the bezier if tweening is enabled
            if (ActiveBezier.Tweening)
            {
                DisplayBezierHandle(ref ActiveBezier.BezierPoints[i]);
            }

            // Run through each bezier point and draw the curve between them
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
    /// Draw the Bézier curve between two points
    /// </summary>
    /// <param name="start">The bezier to start drawing from</param>
    /// <param name="end">The bezier to draw towards</param>
    /// <param name="segmentCount">How many segments make up the curve</param>
    /// <param name="length">The total length of all the line segments</param>
    private void DrawBezierLine(BezierPoint start, BezierPoint end, int segmentCount, out float length)
    {
        // Get all the positions as well as the individual lengths of all the line segments
        Vector3[] allPositions = ActiveBezier.PointsBetweenBeziers(start, end, segmentCount, out length);

        // Iterate through each point at different linearly interpolated values of the curve, drawing a line between
        //    that point and the previous
        for (int i = 1; i < allPositions.Length; i++)
        {
            Vector3 previousPosition = allPositions[i - 1] + ActiveTransform.position;
            Vector3 currentPosition = allPositions[i] + ActiveTransform.position;

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

        // Convert the points and directions into relative positions
        Vector3 bezierPoint = bezier.Point + ActiveTransform.position;
        Vector3 bezierBackPoint = bezierPoint + bezier.BackDir;
        Vector3 bezierForwardPoint = bezierPoint + bezier.ForwardDir;

        // Draw the middle handle
        Handles.color = Color.white;
        bezierPoint = Handles.FreeMoveHandle(bezierPoint, handleSize, Vector3.zero, Handles.DotHandleCap);

        // Draw the line to the backward direction as well as the backward handle
        Handles.color = Color.red;
        bezierBackPoint = 
            Handles.FreeMoveHandle(bezierPoint + bezier.BackDir, handleSize, Vector3.zero, 
                Handles.DotHandleCap);
        Handles.DrawLine(bezierBackPoint, bezierPoint);

        // Draw the line to the forward direction as well as the forward handle
        Handles.color = Color.green;
        bezierForwardPoint = 
            Handles.FreeMoveHandle(bezierPoint + bezier.ForwardDir, handleSize, Vector3.zero, 
                Handles.DotHandleCap);
        Handles.DrawLine(bezierPoint, bezierForwardPoint);

        // Update the bezier's variables based on what the player moved them to
        bezier.Point = bezierPoint - ActiveTransform.position;
        bezier.BackDir =  bezierBackPoint - bezierPoint;
        bezier.ForwardDir = bezierForwardPoint - bezierPoint;
    }
}
