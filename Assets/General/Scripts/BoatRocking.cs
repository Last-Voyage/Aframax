/**********************************************************************************************************************
//
// File Name :         BoatRocking
// Author :            Nick Rice
// Creation Date :     9/16/24
// 
// Brief Description : This script will make the boat rock back and forth
 *********************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatRocking : MonoBehaviour
{
    [Tooltip("The objects on the boat")]
    [NonSerialized]
    public List<GameObject>
        BoatPassengers = new List<GameObject>();

    [NonSerialized][Tooltip("The speed multiplier for the boat")] public float DistanceVariance = .5f;

    private float _moveSpeed = 5f;

    private Vector3 _currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        _currentPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentPosition += new Vector3(Mathf.Sin(Time.time * Mathf.Pow(_moveSpeed, .5f)) * DistanceVariance * Time.deltaTime, 0, 0);

        transform.position = _currentPosition;

        if (BoatPassengers.Count > 0 && BoatPassengers != null)
        {
            foreach (GameObject passenger in BoatPassengers)
            {
                //passenger.transform.position += VelocityMultiplier * Time.deltaTime;
            }
        }
    }
}
