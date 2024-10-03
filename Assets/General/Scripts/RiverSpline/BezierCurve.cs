/*****************************************************************************
// File Name :         BezierCurve.cs
// Author :            Charlie Polonus
// Creation Date :     September 23, 2024
//
// Brief Description : Creates a bezier curve based on a series of points
                       and tweening handles. Math concerning the evaluation
                       of a bezier curve at any given point can be found
                       at https://www.youtube.com/watch?v=aVwxzDHniEw
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a list of points and the methods to generate a bezier curve between them
/// </summary>
public class BezierCurve : MonoBehaviour
{
    [Header("Line Settings")]
    public BezierPoint[] BezierPoints;
    [Tooltip("How intense the curve is relative to the points")]
    public float CurveIntensity = 2;

    [Header("Editor Settings")]
    [Tooltip("The size of the scene handles")]
    [Min(0)] public float HandleSize = 1;
    [Tooltip("How many lines make up each curve (only in the scene view)")]
    [Min(1)] public int CurveSmoothness = 15;

    /// <summary>
    /// Find the linearly interpolated position between two bezier points
    /// </summary>
    /// <param name="start">The bezier point to start from</param>
    /// <param name="end">The bezier point to move towards</param>
    /// <param name="value">The linearly interpolated value to check</param>
    /// <returns>The position at the specified point</returns>
    public Vector2 EvaluateBezierPoint(BezierPoint start, BezierPoint end, float value)
    {
        // The reasoning for these variables can be found in the provided video
        Vector2 P0 = start.Point;
        Vector2 P1 = start.Point + (start.ForwardDir * CurveIntensity);
        Vector2 P2 = end.Point + (end.BackDir * CurveIntensity);
        Vector2 P3 = end.Point;

        // The ^2 and ^3 values of the initial value, to clean up the calculation
        float value2 = Mathf.Pow(value, 2);
        float value3 = Mathf.Pow(value, 3);

        // Linearly interpolate between the four positions to get the final position
        Vector2 P = P0 * (-value3 + 3 * value2 - 3 * value + 1)
            + P1 * (3 * value3 - 6 * value2 + 3 * value)
            + P2 * (-3 * value3 + 3 * value2)
            + P3 * value3;

        return P;
    }

    /// <summary>
    /// Find the linearly interpolated derivative between two bezier points
    /// </summary>
    /// <param name="start">The bezier point to start from</param>
    /// <param name="end">The bezier point to move towards</param>
    /// <param name="value">The linearly interpolated value to check</param>
    /// <returns>The derivative at the specified point</returns>
    public Vector2 EvaluateBezierDerivative(BezierPoint start, BezierPoint end, float value)
    {
        // The reasoning for these variables can be found in the provided video
        Vector2 P0 = start.Point;
        Vector2 P1 = start.Point + (start.ForwardDir * CurveIntensity);
        Vector2 P2 = end.Point + (end.BackDir * CurveIntensity);
        Vector2 P3 = end.Point;

        // The ^2 value of the initial value, to clean up the calculation
        float value2 = Mathf.Pow(value, 2);

        // Linearly interpolate between the four positions to get the final derivative
        Vector2 D = P0 * ((-3 * value2) + (6 * value) - 3)
            + P1 * ((9 * value2) - (12 * value) + 3)
            + P2 * ((-9 * value2) + (6 * value))
            + P3 * (3 * value2);

        return D;
    }

    /// <summary>
    /// Get all positions between two bezier points at a specfied level of detail
    /// </summary>
    /// <param name="start">The bezier to start from</param>
    /// <param name="end">The bezier to move towards</param>
    /// <param name="segments">How many segments make up the curve between the two bezier points</param>
    /// <param name="length">The total of all the line segments</param>
    /// <returns>An array of all the positions between the bezier points</returns>
    public Vector2[] PointsBetweenBeziers(BezierPoint start, BezierPoint end, int segments, out float length)
    {
        // Prepare the array to store all the positions
        Vector2[] allPoints = new Vector2[segments + 1];
        length = 0;

        Vector2 previousPoint = start.Point;

        // Iterate through all the segments of the curve and find the individual positions
        for (int i = 0; i <= segments; i++)
        {
            float lineRatio = i / (float)segments;

            Vector2 currentPoint = EvaluateBezierPoint(start, end, lineRatio);

            // Get the distance between the last line segment, and add it to the total length
            allPoints[i] = currentPoint;
            length += Vector2.Distance(previousPoint, currentPoint);

            previousPoint = currentPoint;
        }
        return allPoints;
    }

    /// <summary>
    /// Get all positions between two bezier points at a specfied level of detail
    /// </summary>
    /// <param name="start">The bezier to start from</param>
    /// <param name="end">The bezier to move towards</param>
    /// <param name="segments">How many segments make up the curve between the two bezier points</param>
    /// <returns>An array of all the positions between the bezier points</returns>
    public Vector2[] PointsBetweenBeziers(BezierPoint start, BezierPoint end, int segments)
    {
        return PointsBetweenBeziers(start, end, segments, out _);
    }

    /// <summary>
    /// Get all derivatives between two bezier points at a specfied level of detail
    /// </summary>
    /// <param name="start">The bezier to start from</param>
    /// <param name="end">The bezier to move towards</param>
    /// <param name="segments">How many segments make up the curve between the two bezier points</param>
    /// <returns>An array of all the derivatives between the bezier points</returns>
    public Vector2[] DerivativesBetweenBeziers(BezierPoint start, BezierPoint end, int segments)
    {
        // Prepare the array to store all the derivatives
        Vector2[] allDerivatives = new Vector2[segments + 1];

        // Iterate through all the segments of the curve and find the individual positions
        for (int i = 0; i <= segments; i++)
        {
            float lineRatio = i / (float)segments;

            Vector2 currentPoint = EvaluateBezierDerivative(start, end, lineRatio);

            allDerivatives[i] = currentPoint;
        }
        return allDerivatives;
    }

    /// <summary>
    /// Get all the points throughout the entire set of bezier points at a specfied level of detail
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <param name="length">The total length of all the lines between bezier points</param>
    /// <returns>An array of all the points across all the bezier points</returns>
    public Vector2[] AllPoints(int segmentsPerCurve, out float length)
    {
        length = 0;

        // Prepares the array to store all the positions
        Vector2[] allPoints = new Vector2[segmentsPerCurve * (BezierPoints.Length - 1) + 1];

        // Iterate through every bezier point and combine all the points in between them
        for (int i = 0; i < BezierPoints.Length - 1; i++)
        {
            Vector2[] curPoints = PointsBetweenBeziers(BezierPoints[i], BezierPoints[i + 1], segmentsPerCurve, out float curLength);

            // If it's the very first point, add it to the list
            if (i == 0)
            {
                allPoints[0] = curPoints[0];
            }

            // Add the points heading towards and the actual point of each of the bezier points
            for (int j = 1; j < curPoints.Length; j++)
            {
                allPoints[i * segmentsPerCurve + j] = curPoints[j];
            }

            length += curLength;
        }

        return allPoints;
    }

    /// <summary>
    /// Get all the points throughout the entire set of bezier points at a specfied level of detail
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <returns>An array of all the points across all the bezier points</returns>
    public Vector2[] AllPoints(int segmentsPerCurve)
    {
        return AllPoints(segmentsPerCurve, out _);
    }

    /// <summary>
    /// Get all the points throughout the entire set of bezier points at a specfied level of detail in 3D space
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <param name="length">The total length of all the lines between bezier points</param>
    /// <returns>An array of all the points across all the bezier points across the X and Z axis</returns>
    public Vector3[] All3dPoints(int segmentsPerCurve, out float length)
    {
        // Gets the 2d positions and prepares the arrays for the 3d positions
        Vector2[] all2dPoints = AllPoints(segmentsPerCurve, out length);
        Vector3[] all3dPoints = new Vector3[all2dPoints.Length];

        // Converts all the 2d positions to 3d ones
        for (int i = 0; i < all2dPoints.Length; i++)
        {
            all3dPoints[i] = new(all2dPoints[i].x, 0, all2dPoints[i].y);
        }

        return all3dPoints;
    }

    /// <summary>
    /// Get all the points throughout the entire set of bezier points at a specfied level of detail in 3D space
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <returns>An array of all the points across all the bezier points across the X and Z axis</returns>
    public Vector3[] All3dPoints(int segmentsPerCurve)
    {
        return All3dPoints(segmentsPerCurve, out _);
    }

    /// <summary>
    /// Get all the derivatives throughout the entire set of bezier points at a specfied level of detail
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <returns>An array of all the derivatives across all the bezier points</returns>
    public Vector2[] AllDerivatives(int segmentsPerCurve)
    {
        // Prepares the array to store all the derivatives
        Vector2[] allDerivatives = new Vector2[segmentsPerCurve * (BezierPoints.Length - 1) + 1];

        // Iterate through every bezier point and combine all the derivatives in between them
        for (int i = 0; i < BezierPoints.Length - 1; i++)
        {
            Vector2[] curPoints = DerivativesBetweenBeziers(BezierPoints[i], BezierPoints[i + 1], segmentsPerCurve);

            // If it's the very first point, add it to the list
            if (i == 0)
            {
                allDerivatives[0] = curPoints[0];
            }

            // Add the derivatives heading towards and the actual derivative of each of the bezier points
            for (int j = 1; j < curPoints.Length; j++)
            {
                allDerivatives[i * segmentsPerCurve + j] = curPoints[j];
            }
        }

        return allDerivatives;
    }

    /// <summary>
    /// Get all the derivatives throughout the entire set of bezier points at a specfied level of detail in 3D space
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <returns>An array of all the derivatives across all the bezier points across the X and Z axis</returns>
    public Vector3[] All3dDerivatives(int segmentsPerCurve)
    {
        // Gets the 2d derivatives and prepares the arrays for the 3d positions
        Vector2[] all2dDerivatives = AllDerivatives(segmentsPerCurve);
        Vector3[] all3dDerivatives = new Vector3[all2dDerivatives.Length];

        // Converts all the 2d derivatives to 3d ones
        for (int i = 0; i < all2dDerivatives.Length; i++)
        {
            all3dDerivatives[i] = new(all2dDerivatives[i].x, 0, all2dDerivatives[i].y);
        }

        return all3dDerivatives;
    }
}

/// <summary>
/// A struct containing information about a point on a bezier curve
/// </summary>
[System.Serializable]
public struct BezierPoint
{
    /// <summary>
    /// A constructor for a bezier point requiring information about its individual points
    /// </summary>
    /// <param name="point">The position the actual point is at</param>
    /// <param name="backDir">The direction backward towards the previous bezier point</param>
    /// <param name="forwardDir">The direction forward towards the next bezier point</param>
    public BezierPoint(Vector2 point, Vector2 backDir, Vector2 forwardDir)
    {
        Point = point;
        BackDir = backDir;
        ForwardDir = forwardDir;
    }

    public Vector2 Point;
    public Vector2 BackDir;
    public Vector2 ForwardDir;

    /// <summary>
    /// Determines whether the bezier points on both sides are the same
    /// </summary>
    /// <param name="a">The left bezier point</param>
    /// <param name="b">The right bezier point</param>
    /// <returns>If the two bezier points are equal</returns>
    public static bool operator ==(BezierPoint a, BezierPoint b)
    {
        return a.Point == b.Point
            && a.BackDir == b.BackDir
            && a.ForwardDir == b.ForwardDir;
    }

    /// <summary>
    /// Determines whether the bezier points on both sides are different
    /// </summary>
    /// <param name="a">The left bezier point</param>
    /// <param name="b">The right bezier point</param>
    /// <returns>If the two bezier points are different</returns>
    public static bool operator !=(BezierPoint a, BezierPoint b)
    {
        return a.Point != b.Point
            || a.BackDir != b.BackDir
            || a.ForwardDir != b.ForwardDir;
    }

    /// <summary>
    /// Allows the comparison of this bezier point to another
    /// </summary>
    /// <param name="obj">The bezier point being compared against</param>
    /// <returns>If the two bezier points are equal</returns>
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    /// <summary>
    /// Allows the comparison of two bezier points
    /// </summary>
    /// <returns>A hash code for the current object</returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}