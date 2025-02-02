/*****************************************************************************
// File Name :         RandomSmoothRotation
// Author :            Tommy Roberts
// Creation Date :     1/31/2025
//
// Brief Description : Just rotates a transform randomly around its the world up with a given 
//                     maximum angle away from the world up axis
*****************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// Contains functionality for rotating an object smoothly around its north pole
/// </summary>
public class RandomSmoothRotation : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 1f; // Speed of rotation
    [SerializeField] private float _maxRotationAngle = 45f; // Maximum rotation angle in either direction
    [SerializeField] private float _minAngleDifference = 15f; // Minimum angle difference between rotations
    [SerializeField] private float _waitTime = 2f; // Time to wait before rotating to a new direction

    private Quaternion _targetRotation; // Target rotation to smoothly rotate towards
    private Quaternion _previousRotation; // Stores the previous target rotation
    private WaitForSeconds _waitTimeWait;

    /// <summary>
    /// Starts the random rotating when the gameobject is enabled
    /// </summary>
    private void Start()
    {
        _waitTimeWait = new WaitForSeconds(_waitTime);

        // Initialize the previous rotation
        _previousRotation = transform.rotation;

        // Start the rotation coroutine
        StartCoroutine(RotateRandomly());
    }

    /// <summary>
    /// Does the rotating mentioned
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateRandomly()
    {
        while (true)
        {
            // Generate a new random target rotation within the specified angle range
            _targetRotation = GetRandomRotation();

            // Smoothly rotate towards the target rotation
            float elapsedTime = 0f;
            Quaternion startRotation = transform.rotation;
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * _rotationSpeed;
                transform.rotation = Quaternion.Slerp(startRotation, _targetRotation, elapsedTime);
                yield return null; // Wait until the next frame
            }

            // Wait for the specified time before rotating again
            yield return _waitTimeWait;

            // Update the previous rotation
            _previousRotation = _targetRotation;
        }
    }

    /// <summary>
    /// generates a random rotation
    /// </summary>
    /// <returns></returns>
    private Quaternion GetRandomRotation()
    {
        Quaternion newRotation;
        float angleDifference;

        do
        {
            // Random angles for Y and Z axes within the specified range
            float randomAngleY = Random.Range(-_maxRotationAngle, _maxRotationAngle);
            float randomAngleZ = Random.Range(-_maxRotationAngle, _maxRotationAngle);

            // Create a rotation that keeps the X-axis pointing down
            // while rotating around the Y and Z axes
            newRotation = Quaternion.Euler(0, randomAngleY, randomAngleZ - 90f);

            // Calculate the angle difference between the new rotation and the previous rotation
            angleDifference = Quaternion.Angle(_previousRotation, newRotation);
        }
        while (angleDifference < _minAngleDifference); // Repeat until the angle difference is large enough

        return newRotation;
    }
}