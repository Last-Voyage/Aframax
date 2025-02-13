/*****************************************************************************
// File Name :         LookAtMoveDirection.cs
// Author :            Tommy Roberts
// Creation Date :     11/21/2024
//
// Brief Description : holds functionality to rotate object towards the direction it is moving.
*****************************************************************************/
using UnityEngine;

/// <summary>
/// rotates the gameobject toward the direction it is moving based on its position during the previous call of update
/// </summary>
public class LookAtMoveDirection : MonoBehaviour
{
    private Vector3 lastPosition; // Store the last frame's position
    [Tooltip("Speed at which the object rotates")]
    [SerializeField] private float rotationSpeed = 10f; // Speed at which the object rotates

    /// <summary>
    /// sets first previous position
    /// </summary>
    private void Start()
    {
        // Initialize the last position
        lastPosition = transform.position;
    }

    /// <summary>
    /// changes the rotation of the the object to look at the direction it is moving toward
    /// </summary>
    private void Update()
    {
        // Calculate the velocity (difference in position over time)
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;

        // Update the last position for the next frame
        lastPosition = transform.position;

        // Check if the object is moving
        if (velocity.sqrMagnitude > 0.01f) // Ignore very small movements
        {
            // Calculate the target rotation so that 'up' faces the velocity direction
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.forward);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
