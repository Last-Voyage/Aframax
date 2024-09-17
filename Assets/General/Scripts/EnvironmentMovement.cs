/**********************************************************************************************************************
//
// File Name :         EnvironmentMovement
// Author :            Nick Rice
// Creation Date :     9/14/24
// 
// Brief Description : This script will move the environment pieces in different directions and speeds
 *********************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UIElements;

public class EnvironmentMovement : MonoBehaviour
{
    #region Directional Variables
    private static Vector3 ForwardDirection = Vector3.forward;
    private static Vector3 BackwardDirection = Vector3.back;
    private static Vector3 LeftDirection = Vector3.left;
    private static Vector3 RightDirection = Vector3.right;
    private static Vector3 ForwardLeftDirection = Vector3.forward + Vector3.left;
    private static Vector3 ForwardRightDirection = Vector3.forward + Vector3.right;
    private static Vector3 BackLeftDirection = Vector3.back + Vector3.left;
    private static Vector3 BackRightDirection = Vector3.back + Vector3.right;

    [Tooltip("Named pointers for the background direction")]
    public enum EDirectionalInserts
    {
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }

    [Tooltip("Array that holds directions for the background")]
    private Vector3[] _directionalMovements =
        { ForwardDirection, BackwardDirection, LeftDirection, RightDirection,
        ForwardLeftDirection, ForwardRightDirection, BackLeftDirection, BackRightDirection };
    #endregion

    [Tooltip("The pointer for the background direction")] private int _currentDirectional = 
        (int)EDirectionalInserts.Backward;
    [NonSerialized][Tooltip("The speed multiplier for the background")] public float VelocityMultiplier = 2f;


    // Uses a test coroutine to change the speed
    private void Start()
    {
        StartCoroutine(TestSpeedChange());
    }

    // Updates the movement of the background
    void FixedUpdate()
    {
        gameObject.transform.position += _directionalMovements[_currentDirectional] *
                VelocityMultiplier * Time.deltaTime;
    }

    // This is a test coroutine to change the speed
    private IEnumerator TestSpeedChange()
    {
        yield return new WaitForSeconds(5f);
        EnvironmentDirectionChanger(EDirectionalInserts.BackwardLeft);
        VelocityMultiplier = 3f;
    }

    /// <summary>
    /// A function that changes the way the environment moves
    /// </summary>
    /// <param name="direction">The enum that will change the direction</param>
    public void EnvironmentDirectionChanger(EDirectionalInserts direction)
    {
        _currentDirectional = (int)direction;
    }
}
