/**********************************************************************************************************************
// File Name :          LightShift.cs
// Author :             Andrew Stapay
// Creation Date :      2/4/25
//
// Brief description :  Triggers tentacle movement for the Slytherin Horror Moment
**********************************************************************************************************************/
using UnityEngine;

/// <summary>
/// Moves the tentacle when passed through
/// </summary>
public class TentacleMover : MonoBehaviour
{
    [SerializeField] private SlytherinTentacleBehavior _tentacle;


    /// <summary>
    /// Called when the player makes contact with the associated trigger
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tentacle.MoveAway();
            StartCoroutine(_tentacle.DestroyTentacle(3f));
            //Destroy(this.gameObject, 1);
        }
    }
}