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
    [Tooltip("Whether or not to be editing the spline")]
    public bool Tweening = true;

    /// <summary>
    /// Find the linearly interpolated position between two bezier points
    /// </summary>
    /// <param name="start">The bezier point to start from</param>
    /// <param name="end">The bezier point to move towards</param>
    /// <param name="value">The linearly interpolated value to check</param>
    /// <returns>The position at the specified point</returns>
    public Vector3 EvaluateBezierPoint(BezierPoint start, BezierPoint end, float value)
    {
        // The reasoning for these variables can be found in the provided video
        Vector3 P0 = start.Point;
        Vector3 P1 = start.Point + (start.ForwardDir * CurveIntensity);
        Vector3 P2 = end.Point + (end.BackDir * CurveIntensity);
        Vector3 P3 = end.Point;

        // The ^2 and ^3 values of the initial value, to clean up the calculation
        float value2 = Mathf.Pow(value, 2);
        float value3 = Mathf.Pow(value, 3);

        // Linearly interpolate between the four positions to get the final position
        Vector3 P = P0 * (-value3 + 3 * value2 - 3 * value + 1)
            + P1 * (3 * value3 - 6 * value2 + 3 * value)
            + P2 * (-3 * value3 + 3 * value2)
            + P3 * value3;

        return P;
    }

    [System.Obsolete]
    /// <summary>
    /// Calculates the distance along the line at a certain point
    /// </summary>
    /// <param name="start">The starting bezier point</param>
    /// <param name="end">The ending bezier point</param>
    /// <param name="pointIndex">The point to be checking up until</param>
    /// <returns>The distance along the line</returns>
    public float DistanceAlongLine(BezierPoint start, BezierPoint end, int pointIndex)
    {
        if (pointIndex == 0)
        {
            return 0;
        }

        float totalDistance = 0;
        Vector3 previousPoint = EvaluateBezierPoint(start, end, 0);
        for (int i = 1; i < pointIndex; i++)
        {
            Vector3 currentPoint = EvaluateBezierPoint(start, end, i / (float)CurveSmoothness);

            totalDistance += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return totalDistance;
    }

    /// <summary>
    /// Find the linearly interpolated derivative between two bezier points
    /// </summary>
    /// <param name="start">The bezier point to start from</param>
    /// <param name="end">The bezier point to move towards</param>
    /// <param name="value">The linearly interpolated value to check</param>
    /// <returns>The derivative at the specified point</returns>
    public Vector3 EvaluateBezierDerivative(BezierPoint start, BezierPoint end, float value)
    {
        // The reasoning for these variables can be found in the provided video
        Vector3 P0 = start.Point;
        Vector3 P1 = start.Point + (start.ForwardDir * CurveIntensity);
        Vector3 P2 = end.Point + (end.BackDir * CurveIntensity);
        Vector3 P3 = end.Point;

        // The ^2 value of the initial value, to clean up the calculation
        float value2 = Mathf.Pow(value, 2);

        // Linearly interpolate between the four positions to get the final derivative
        Vector3 D = P0 * ((-3 * value2) + (6 * value) - 3)
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
    public Vector3[] PointsBetweenBeziers(BezierPoint start, BezierPoint end, int segments, out float length)
    {
        // Prepare the array to store all the positions
        Vector3[] allPoints = new Vector3[segments + 1];
        length = 0;

        Vector3 previousPoint = start.Point;

        // Iterate through all the segments of the curve and find the individual positions
        for (int i = 0; i <= segments; i++)
        {
            float lineRatio = i / (float)segments;

            Vector3 currentPoint = EvaluateBezierPoint(start, end, lineRatio);

            // Get the distance between the last line segment, and add it to the total length
            allPoints[i] = currentPoint;
            length += Vector3.Distance(previousPoint, currentPoint);

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
    public Vector3[] PointsBetweenBeziers(BezierPoint start, BezierPoint end, int segments)
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
    public Vector3[] DerivativesBetweenBeziers(BezierPoint start, BezierPoint end, int segments)
    {
        // Prepare the array to store all the derivatives
        Vector3[] allDerivatives = new Vector3[segments + 1];

        // Iterate through all the segments of the curve and find the individual positions
        for (int i = 0; i <= segments; i++)
        {
            float lineRatio = i / (float)segments;

            Vector3 currentPoint = EvaluateBezierDerivative(start, end, lineRatio);

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
    public Vector3[] AllPoints(int segmentsPerCurve, out float length)
    {
        length = 0;

        // Prepares the array to store all the positions
        Vector3[] allPoints = new Vector3[segmentsPerCurve * (BezierPoints.Length - 1) + 1];

        // Iterate through every bezier point and combine all the points in between them
        for (int i = 0; i < BezierPoints.Length - 1; i++)
        {
            Vector3[] curPoints = PointsBetweenBeziers(BezierPoints[i], BezierPoints[i + 1], segmentsPerCurve, out float curLength);

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
    public Vector3[] AllPoints(int segmentsPerCurve)
    {
        return AllPoints(segmentsPerCurve, out _);
    }

    /// <summary>
    /// Get all the derivatives throughout the entire set of bezier points at a specfied level of detail
    /// </summary>
    /// <param name="segmentsPerCurve">How many segments each curve is split into</param>
    /// <returns>An array of all the derivatives across all the bezier points</returns>
    public Vector3[] AllDerivatives(int segmentsPerCurve)
    {
        // Prepares the array to store all the derivatives
        Vector3[] allDerivatives = new Vector3[segmentsPerCurve * (BezierPoints.Length - 1) + 1];

        // Iterate through every bezier point and combine all the derivatives in between them
        for (int i = 0; i < BezierPoints.Length - 1; i++)
        {
            Vector3[] curPoints = DerivativesBetweenBeziers(BezierPoints[i], BezierPoints[i + 1], segmentsPerCurve);

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
    /// Gets the length of the active bezier
    /// </summary>
    /// <returns>The total length of the bezier at the current level of detail</returns>
    public float GetLengthOfLine()
    {
        AllPoints(CurveSmoothness, out float length);
        return length;
    }

    /// <summary>
    /// Gets the position along the line at a certain float value
    /// </summary>
    /// <param name="value">The percentage value along the line to be determining</param>
    /// <param name="forward">The forward derivative along the line</param>
    /// <param name="worldPosition">Whether or not the returned position should be in world position</param>
    /// <returns>The position along the line at the given value</returns>
    public Vector3 GetPositionAlongSpline(float value, out Vector3 forward, bool worldPosition = false)
    {
        // Clamp the value between 0 and 100%
        value = Mathf.Clamp01(value);

        // Get the length that the boat needs to travel along the line
        float length = GetLengthOfLine();
        float distanceOverLine = value * length;

        // Iterate through each line segment and find the one that the point is in
        float previousLength = 0;
        float totalLength = 0;
        for (int i = 0; i < BezierPoints.Length - 1; i++)
        {
            // Get the current length of the "active" segment and add it to the main distance
            PointsBetweenBeziers(BezierPoints[i], BezierPoints[i + 1], CurveSmoothness, out float currentLength);
            totalLength += currentLength;

            // If the distance value is less than the current length, it will be in this segment
            if (distanceOverLine <= totalLength)
            {
                // Find the value along and position within the line segment
                float valueOnSegment = (distanceOverLine - previousLength) / currentLength;
                Vector3 positionOnSegment = EvaluateBezierPoint(BezierPoints[i], BezierPoints[i + 1], valueOnSegment);

                // Determine the derivative at the point along the line
                Vector3 derivativeOnSegment = EvaluateBezierDerivative(BezierPoints[i], BezierPoints[i + 1], valueOnSegment);
                forward = derivativeOnSegment;

                // Account for 3D space
                return positionOnSegment + (worldPosition ? transform.position : Vector3.zero);
            }
            previousLength = totalLength;
        }

        // Edge case
        forward = Vector3.zero;
        return worldPosition ? transform.position : Vector3.zero;
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
    public BezierPoint(Vector3 point, Vector3 backDir, Vector3 forwardDir)
    {
        Point = point;
        BackDir = backDir;
        ForwardDir = forwardDir;
    }

    public Vector3 Point;
    public Vector3 BackDir;
    public Vector3 ForwardDir;

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