/*****************************************************************************
// File Name :         AttractionBehavior
// Author :            Tommy Roberts
// Creation Date :     10/28/2024
//
// Brief Description : Controls attraction behavior for procedural animation
*****************************************************************************/
using UnityEngine;

/// <summary>
/// Guides a transform toward a target, but does not get too far away from parent
/// </summary>
public class AttractionBehavior : MonoBehaviour
{
    [SerializeField] private Transform _target; // The target GameObject
    [SerializeField] private  Transform _parent; // The object's parent
    [SerializeField] private  float _attractionRange = 5f; // The spherical range within which the object moves towards the target
    [SerializeField] private  float _maxDistanceFromParent = 3f; // Maximum distance from parent
    [SerializeField] private float _stopFollowDistance = 2f;
    [SerializeField] private float _followSpeed = 2f; // Speed at which the object moves
    [SerializeField] private  float _rotateSpeed = 2f;

    /// <summary>
    /// Contains the functionality to slowly move toward target when in range and then retract when not in range
    /// </summary>
    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        if (distanceToTarget <= _attractionRange && distanceToTarget >= _stopFollowDistance)
        {
            // Move towards the target within the max distance from the parent
            Vector3 directionToTarget = (_target.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToTarget * _followSpeed * Time.deltaTime;

            if (Vector3.Distance(newPosition, _parent.position) <= _maxDistanceFromParent)
            {
                transform.position = newPosition;
                // Check if the direction is valid (to avoid errors when the object is exactly above/below the target)
                if (directionToTarget.sqrMagnitude > 0.01f)
                {
                    // Calculate the target rotation
                    Vector3 targetPosition = new Vector3(_target.position.x, transform.root.position.y, _target.transform.position.z);
                    Vector3 rootPos = transform.root.position;
                    rootPos.y = 0;

                    // Calculate the target rotation to look at the target position
                    Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.root.position);

                    // Smoothly rotate towards the target rotation on the y-axis only
                    transform.root.rotation = Quaternion.Slerp(transform.root.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            // Move back to the parent position
            Vector3 directionToTarget = (_parent.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToTarget * _followSpeed * Time.deltaTime;

            if(Vector3.Distance(newPosition, _parent.position) > .1f)
            {
                transform.position = newPosition;
            }
        }
    }
}