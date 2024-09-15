using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class BoatMovement : MonoBehaviour
{
    #region Directional Variables
    private static Vector3 ForwardDirection = Vector3.forward;
    private static Vector3 BackwardDirection = Vector3.back;
    private static Vector3 LeftDirection = Vector3.left;
    private static Vector3 RightDirection = Vector3.right;
    #endregion

    private enum EDirectionalInserts
    {
        Forward,
        Backward,
        Left,
        Right
    }

    private Vector3[] _directionalMovements = { ForwardDirection, BackwardDirection, LeftDirection, RightDirection };

    private int _currentDirectional = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
