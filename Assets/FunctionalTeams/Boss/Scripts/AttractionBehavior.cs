using UnityEngine;

public class AttractionBehavior : MonoBehaviour
{
    public Transform target; // The target GameObject
    public Transform parent; // The object's parent
    public float attractionRange = 5f; // The spherical range within which the object moves towards the target
    public float maxDistanceFromParent = 3f; // Maximum distance from parent
    public float stopFollowDistance = 2f;
    public float moveSpeed = 2f; // Speed at which the object moves
    public float rotationSpeed = 2f;

    private bool isInAttractionRange = false;
    private Vector3 originalLocalPosition; // The object's original position relative to its parent

    private void Start()
    {
        originalLocalPosition = Vector3.zero;
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float distanceToParent = Vector3.Distance(transform.localPosition, Vector3.zero);

        if (distanceToTarget <= attractionRange && distanceToTarget >= stopFollowDistance)
        {
            isInAttractionRange = true;

            // Move towards the target within the max distance from the parent
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToTarget * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(newPosition, parent.position) <= maxDistanceFromParent)
            {
                transform.position = newPosition;
                // Check if the direction is valid (to avoid errors when the object is exactly above/below the target)
                if (directionToTarget.sqrMagnitude > 0.01f)
                {
                    // Calculate the target rotation
                    Vector3 targetPosition = new Vector3(target.position.x, transform.root.position.y, target.transform.position.z);
                    Vector3 rootPos = transform.root.position;
                    rootPos.y = 0;

                    // Calculate the target rotation to look at the target position
                    Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.root.position);

                    // Smoothly rotate towards the target rotation on the y-axis only
                    transform.root.rotation = Quaternion.Slerp(transform.root.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            // Move back to the parent position
            isInAttractionRange = false;
            Vector3 directionToTarget = (parent.position - transform.position).normalized;
            Vector3 newPosition = transform.position + directionToTarget * moveSpeed * Time.deltaTime;

            if(Vector3.Distance(newPosition, parent.position) > .1f)
            {
                transform.position = newPosition;
            }
        }
    }
}
