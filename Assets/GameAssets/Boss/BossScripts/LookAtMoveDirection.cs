using UnityEngine;

public class LookAtMoveDirection : MonoBehaviour
{
    private Vector3 lastPosition; // Store the last frame's position
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
