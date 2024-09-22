/**********************************************************************************************************************
// File Name :         BoatMover.cs
// Author :            Nick Rice, Alex Kalscheur
// Creation Date :     9/16/24
// 
// Brief Description : Controls the boat and keeps entities from falling off boat
**********************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMover : MonoBehaviour
{
    private float yOffset;

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

    public List<GameObject> TrainPassengers { get; set; }

    [Tooltip("The pointer for the background direction")]
    private int _currentDirectional = (int)EDirectionalInserts.Forward;

    [NonSerialized][Tooltip("The speed multiplier for the background")] 
    public float VelocityMultiplier = 2;

    /// <summary>
    /// At the start of the script, TestSpeedChange coroutine is started
    /// </summary>
    private void Start()
    {
        TrainPassengers = new List<GameObject>();
        StartCoroutine(TestSpeedChange());
        yOffset = gameObject.transform.position.y;
    }

    /// <summary>
    /// Moves the boat and the objects on it
    /// </summary>
    void FixedUpdate()
    {
        //gets initial movement position
        gameObject.transform.position += _directionalMovements[_currentDirectional] *
            VelocityMultiplier * Time.deltaTime;

        //determines y position based on (sample) sine wave
        Vector3 wavePosition = new Vector3(
            gameObject.transform.position.x,
            MathF.Sin(Time.time * 2) * 20 * Time.deltaTime,
            gameObject.transform.position.z
        );
        gameObject.transform.position = wavePosition;

        //finds rotation based on the wave
        gameObject.transform.rotation = new Quaternion(
            -MathF.Cos(Time.time * 2) * Time.deltaTime,
            gameObject.transform.rotation.y,
            gameObject.transform.rotation.z,
            gameObject.transform.rotation.w
        );

        //adjusts passenger postion to stay on boat
        if (TrainPassengers.Count != 0 && TrainPassengers != null)
        {
            foreach (GameObject passenger in TrainPassengers)
            {
                passenger.transform.position += _directionalMovements[_currentDirectional] *
                    VelocityMultiplier * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// This is a test coroutine to change the speed of the boat
    ///     After 5 seconds, the velocity multiplier is changed from 2 to 3
    ///     5 seconds later, that is reverted back to 2
    /// </summary>
    private IEnumerator TestSpeedChange()
    {
        yield return new WaitForSeconds(5f);
        BoatDirectionChanger(EDirectionalInserts.ForwardRight);
        VelocityMultiplier = 3f;

        yield return new WaitForSeconds(5f);
        BoatDirectionChanger(EDirectionalInserts.Forward);
        VelocityMultiplier = 2f;
    }

    public void BoatDirectionChanger(EDirectionalInserts direction)
    {
        _currentDirectional = (int)direction;
    }
}
