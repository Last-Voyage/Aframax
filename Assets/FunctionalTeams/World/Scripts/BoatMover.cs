/**********************************************************************************************************************
// File Name :         BoatMover.cs
// Author :            Alex Kalscheur
// Contrubuter :       Charlie Polonus
// Creation Date :     10/10/24
// 
// Brief Description : Moves boat along and between river splines
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the boat along and between river splines
/// </summary>
public class BoatMover : MonoBehaviour
{
    [Tooltip("The current spline that the boat is moving along")]
    private BezierCurve _currentSpline;
    
    [Tooltip("After the current spline is completed, this will become the current spline")]
    private BezierCurve _nextSpline;
    
    [Tooltip("Length of the spline")] 
    private float _splineLength;
    
    [Tooltip("Shorthand way to represent the BezierCurve for the current spline")] 
    private BezierCurve _curve;
    
    [Tooltip("Used to determine the orientation of the boat")] 
    private Vector3 _newForwardVector;

    [Tooltip("Determines if the boat is moving")]
    [SerializeField] private bool _isMoving = false;
    
    [Tooltip("How much of the current spline has been completed")] 
    private float _percentOfSpline = 0;

    [Tooltip("Arbitrary float for adjusting the speed of the boat")]
    [SerializeField] private float _speedModifier;

    /// <summary>
    /// Every frame, the boat will be moved
    /// </summary>
    private void Update()
    {
        if (_isMoving)
        {
            MoveBoat();
        }
    }

    /// <summary>
    /// Takes the current spline and moves the boat along it
    ///     Takes into account the length of the spline, time, and a speed modifier to determine the boat's position
    ///     Will eventually also rotate the boat using the derivative of the curve
    /// </summary>
    private void MoveBoat()
    {
        _percentOfSpline += Time.deltaTime * _speedModifier * _splineLength / 100;
        CheckSplineChange();
        Vector3 newPositionOnSpline = _curve.GetPositionAlongSpline(_percentOfSpline, out _newForwardVector, true);

        // Update the position and direction of the moving object
        transform.position = Vector3.Lerp(transform.position, newPositionOnSpline, 0.75f * Time.deltaTime);
        transform.forward += (_newForwardVector - transform.forward).normalized * Time.deltaTime;
    }

    /// <summary>
    /// Sets the current spline, its length and the curve component for shorthand
    /// </summary>
    /// <param name="spline">the spline</param>
    public void SetCurrentSpline(BezierCurve spline)
    {
        _curve = spline;
        _splineLength = _curve.GetLengthOfLine();
    }

    /// <summary>
    /// Sets the spline that will be used after the current spline is completed
    /// </summary>
    /// <param name="spline">the spline</param>
    public void SetNextSpline (BezierCurve spline)
    {
        _nextSpline = spline;
    }

    /// <summary>
    /// Checks to see if the boat has completed the current spline
    /// If the current spline has been completed, the chunks will update and the boat will continue to the next chunk
    /// </summary>
    private void CheckSplineChange()
    {
        if (_percentOfSpline >= 1) {
            _percentOfSpline -= 1;
            EnvironmentManager.Instance.SendChangeTheChunk()?.Invoke();
        }
    }
}
