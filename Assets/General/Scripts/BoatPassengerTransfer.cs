/**********************************************************************************************************************
// File Name :         BoatPassengerTransfer.cs
// Author :            Nick Rice, Alex Kalscheur
// Creation Date :     9/16/24
// 
// Brief Description : Handles keeping player (and other objects) on the boat as said boat moves
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPassengerTransfer : MonoBehaviour
{
    private BoatMover _moveVessel;
    private List<GameObject> _objOnVessel;

    /// <summary>
    /// When enabled, moveVessel gets set to the boat itself
    /// </summary>
    private void Awake()
    {
        _moveVessel = GetComponentInParent<BoatMover>();
    }

    /// <summary>
    /// When an object collides with the boat, that object is added to the array of items on the boat
    /// </summary>
    /// <param name="collision">Collision with boat</param>
    private void OnCollisionEnter(Collision collision)
    {
        _moveVessel.AddTrainPassenger(collision);
    }

    /// <summary>
    /// When an object is no longer contacting the boat, that object is removed from the array of items on the boat
    /// </summary>
    /// <param name="collision">Collision with boat</param>
    private void OnCollisionExit(Collision collision)
    {
        _moveVessel.RemoveTrainPassenger(collision);
    }
}
