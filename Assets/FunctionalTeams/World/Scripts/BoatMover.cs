/**********************************************************************************************************************
// File Name :         BoatMover.cs
// Author :            Alex Kalscheur
// Creation Date :     10/10/24
// 
// Brief Description : Moves boat along and between river splines
**********************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatMover : MonoBehaviour
{
    [Tooltip("The current spline that the boat is moving along")]
    private RiverSpline _currentSpline;
    
    [Tooltip("After the current spline is completed, this will become the current spline")]
    private RiverSpline _nextSpline;
    
    [Tooltip("Length of the spline")] 
    private float _splineLength;
    
    [Tooltip("Shorthand way to represent the BezierCurve for the current spline")] 
    private BezierCurve curve;
    
    [Tooltip("Used to determine the orientation of the boat")] 
    private Vector3 forward;
    
    [Tooltip("How much of the current spline has been completed")] 
    private float _percentOfSpline = 0;

    [Tooltip("Arbitrary float for adjusting the speed of the boat")]
    [SerializeField] private float _speedModifier;

    /// <summary>
    /// Every frame, the boat will be moved
    /// </summary>
    private void Update()
    {
        moveBoat();
    }

    /// <summary>
    /// Takes the current spline and moves the boat along it
    ///     Takes into account the length of the spline, time, and a speed modifier to determine the boat's position
    ///     Will eventually also rotate the boat using the derivative of the curve
    /// </summary>
    private void moveBoat()
    {
        _percentOfSpline += Time.deltaTime * _speedModifier * _splineLength / 100;
        CheckSplineChange();
        Vector3 position = curve.GetPositionAlongSpline(_percentOfSpline, out forward);
        gameObject.transform.position = position;
        gameObject.transform.forward = forward;
    }

    /// <summary>
    /// Sets the current spline, its length and the curve component for shorthand
    /// </summary>
    /// <param name="spline">the spline</param>
    public void SetCurrentSpline(RiverSpline spline)
    {
        _currentSpline = spline;
        curve = _currentSpline.GetComponent<BezierCurve>();
        _splineLength = curve.GetLengthOfLine();
    }

    /// <summary>
    /// Sets the spline that will be used after the current spline is completed
    /// </summary>
    /// <param name="spline">the spline</param>
    public void SetNextSpline (RiverSpline spline)
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
