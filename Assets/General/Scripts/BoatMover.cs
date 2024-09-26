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
using UnityEngine.UIElements;

public class BoatMover : MonoBehaviour
{
    private float _yOffset;
    private List<GameObject> _trainPassengers { get; set; }
    [NonSerialized][Tooltip("The speed multiplier for the background")] public float VelocityMultiplier = 2;

    /// <summary>
    /// Instantiates _trainPassengers and yOffset
    /// </summary>
    private void Awake()
    {
        _trainPassengers = new List<GameObject>();
        _yOffset = gameObject.transform.position.y;
    }

    /// <summary>
    /// At the start of the script, TestSpeedChange and MoveBoat coroutines are started
    /// </summary>
    private void Start()
    {
        StartCoroutine(TestSpeedChange());
    }

    /// <summary>
    /// Moves boat, all passengers, and runs the sample wave
    /// </summary>
    private void FixedUpdate()
    {
        MoveBoat();
        MovePassengers();
        SampleWave();
    }

    /// <summary>
    /// This is a test coroutine to change the speed of the boat
    ///     After 5 seconds, the velocity multiplier is changed from 2 to 3
    ///     5 seconds later, that is reverted back to 2
    /// </summary>
    private IEnumerator TestSpeedChange()
    {
        yield return new WaitForSeconds(5f);
        gameObject.transform.Rotate(new Vector3(0, 45, 0));
        //_trainPassengers[0].transform.position += new Vector3(-1f, .5f, 1f);
        RotatePassengers(new Vector3(0, 45, 0));
        VelocityMultiplier = 3f;

        yield return new WaitForSeconds(5f);
        gameObject.transform.Rotate(new Vector3(0, -45, 0));
        RotatePassengers(new Vector3(0, -45, 0));
        VelocityMultiplier = 2f;
    }

    /// <summary>
    /// Moves the boat and the objects on it
    /// </summary>
    private void MoveBoat()
    {
        //gets initial movement position
        gameObject.transform.position += transform.forward *
            VelocityMultiplier * Time.fixedDeltaTime;
    }

    private void MovePassengers()
    {
        //adjusts passenger postion to stay on boat
        if (_trainPassengers.Count != 0 && _trainPassengers != null)
        {
            foreach (GameObject passenger in _trainPassengers)
            {
                passenger.transform.position += gameObject.transform.forward *
                    VelocityMultiplier * Time.fixedDeltaTime;
            }
        }
    }

    private void RotatePassengers(Vector3 rotationVector)
    {
        if (_trainPassengers.Count != 0 && _trainPassengers != null)
        {
            foreach (GameObject passenger in _trainPassengers)
            {
                passenger.transform.Rotate(rotationVector);
            }
        }
    }

    private void SampleWave()
    {
        //determines y position based on (sample) sine wave
        Vector3 wavePosition = new Vector3(
            gameObject.transform.position.x,
            (MathF.Sin(Time.time * 2) * 20 * Time.fixedDeltaTime) + _yOffset,
            gameObject.transform.position.z
        );
        gameObject.transform.position = wavePosition;

        //finds rotation based on the wave
        gameObject.transform.rotation = new Quaternion(
            -MathF.Cos(Time.time * 2) * Time.fixedDeltaTime,
            gameObject.transform.rotation.y,
            gameObject.transform.rotation.z,
            gameObject.transform.rotation.w
        );
    }

    public void AddTrainPassenger(Collision collision)
    {
        _trainPassengers.Add(collision.gameObject);
    }

    public void RemoveTrainPassenger(Collision collision)
    {
        _trainPassengers.Remove(collision.gameObject);
    }
}
