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

    private void Start()
    {
        _tentacle.MoveAway();
        _tentacle.DestroyTentacle(3f);
        Destroy(this.gameObject, 3f);
    }

    /// <summary>
    /// Called when the player makes contact with the associated trigger
    /// </summary>
    /// <param name="other"> Information about the other collider in the collision </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _tentacle.MoveAway();
            _tentacle.DestroyTentacle(2.5f);
            Destroy(this.gameObject, 2.5f);
        }
    }
}