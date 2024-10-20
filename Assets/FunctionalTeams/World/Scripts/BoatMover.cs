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
    private float _percentOfSpline;
    [SerializeField] private float _speedModifier;

    private void Start()
    {
        _currentSpline = FindObjectOfType<RiverSpline>();
        _percentOfSpline = 0;
    }

    private void Update()
    {
        moveBoat();
    }

    private void moveBoat()
    {
        BezierCurve curve = _currentSpline.GetComponent<BezierCurve>();
        float length = curve.GetLengthOfLine();
        Vector3 rotation;

        _percentOfSpline += Time.deltaTime * _speedModifier * length / 100;
        Debug.Log(_percentOfSpline);
        Vector3 position = curve.GetPositionAlongSpline(_percentOfSpline, out rotation);
        Debug.Log("Calculated: " + position.x + ", " + position.y + ", " + position.z);
        gameObject.transform.position = position;
        Debug.Log("Actual: " + gameObject.transform.position.x + ", " + gameObject.transform.position.y + ", " + gameObject.transform.position.z);
    }
}
