using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPassengerTransfer : MonoBehaviour
{
    private BoatMover moveVessel;
    private List<GameObject> objOnVessel;

    private void OnEnable()
    {
        moveVessel = GetComponentInParent<BoatMover>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        moveVessel.TrainPassengers.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        moveVessel.TrainPassengers.Remove(collision.gameObject);
    }
}
