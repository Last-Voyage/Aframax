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
    private RiverSpline _currentSpline;
    private RiverSpline _nextSpline;
    private float _percentOfSpline = 0;
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
        BezierCurve curve = _currentSpline.GetComponent<BezierCurve>();
        float length = curve.GetLengthOfLine();
        Vector3 rotation;

        _percentOfSpline += Time.deltaTime * _speedModifier * length / 100;
        //Debug.Log(_percentOfSpline);
        Vector3 position = curve.GetPositionAlongSpline(_percentOfSpline, out rotation);
        gameObject.transform.position = position;
        //gameObject.transform.Rotate(rotation);
    }

    public void SetCurrentSpline(RiverSpline spline)
    {
        _currentSpline = spline;
    }

    public void SetNextSpline (RiverSpline spline)
    {
        _nextSpline = spline;
    }
}
