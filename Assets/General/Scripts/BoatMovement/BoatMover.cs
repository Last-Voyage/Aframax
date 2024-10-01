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
    private float _yOffset;
    private bool _isMoving;
    private List<GameObject> _trainPassengers { get; set; }
    [NonSerialized][Tooltip("The speed multiplier for the background")] private float VelocityMultiplier = 2;

    /// <summary>
    /// Instantiates _trainPassengers and yOffset
    /// </summary>
    private void Awake()
    {
        _trainPassengers = new List<GameObject>();
        _yOffset = gameObject.transform.position.y;
        Application.targetFrameRate = 144;
        _isMoving = true;
    }

    /// <summary>
    /// Moves boat, all passengers, and runs the sample wave
    /// </summary>
    private void Update()
    {
        if(_isMoving)
        {
            MoveBoat();
            MovePassengers();
        }
    }

    /// <summary>
    /// Moves the boat and the objects on it
    /// </summary>
    private void MoveBoat()
    {
        gameObject.transform.position += transform.forward * VelocityMultiplier * Time.deltaTime;
    }

    /// <summary>
    /// Moves passengers with the boat to keep all in sync
    /// </summary>
    private void MovePassengers()
    {
        foreach (GameObject passenger in _trainPassengers)
        {
            passenger.transform.position += gameObject.transform.forward *
                VelocityMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// CURRENTLY UNUSED, WILL LIKELY COME BACK OR BE REWORKED LATER
    /// Rotates passengers with the boat
    /// </summary>
    /// <param name="rotationVector">should be the same vector that the boat is rotating on</param>
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

    /// <summary>
    /// to be called in BoatPassengerTransfer.cs, allows for the addition of an object to the _trainPassengers List
    /// </summary>
    /// <param name="collider">Object that you are adding as a passenger</param>
    public void AddTrainPassenger(GameObject collider)
    {
        _trainPassengers.Add(collider);
    }

    /// <summary>
    /// to be called in BoatPassengerTransfer.cs, allows for the removal of an object to the _trainPassengers List
    /// </summary>
    /// <param name="collider">Object that you are removing as a passenger</param>
    public void RemoveTrainPassenger(GameObject collider)
    {
        _trainPassengers.Remove(collider);
    }
}
