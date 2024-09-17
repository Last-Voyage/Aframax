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

    [Tooltip("The speed multiplier for the boat")] public float VelocityMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BoatPassengers.Count > 0 && BoatPassengers != null)
        {
            foreach (GameObject passenger in BoatPassengers)
            {
                //passenger.transform.position += VelocityMultiplier * Time.deltaTime;
            }
        }
    }
}
