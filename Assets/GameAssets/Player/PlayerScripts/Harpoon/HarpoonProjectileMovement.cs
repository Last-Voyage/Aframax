/*****************************************************************************
// File Name :         HarpoonProjectileMovement.cs
// Author :            Ryan Swanson
// Contributors:       David Henvick, Alex Kalscheur
// Creation Date :     10/28/2024
//
// Brief Description : Controls the movement of the harpoon projectile
*****************************************************************************/

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the movement of the harpoon projectile fired from the gun
/// </summary>
public class HarpoonProjectileMovement : MonoBehaviour
{

    private Transform _movingObjects;

    private void OnEnable()
    {
        _movingObjects = FindObjectOfType<BoatMover>().gameObject.transform;
    }

    /// <summary>
    /// Fires the harpoon and sets the position and rotation. Is called by the harpoon gun
    /// </summary>
    /// <param name="startLocation"> The location that the harpoon begins at </param>
    /// <param name="startDirection"> The direction the harpoon is fired in </param>
    public void LaunchHarpoon(Vector3 startLocation, Vector3 startDirection)
    {
        transform.position = startLocation;
        transform.LookAt(transform.position + startDirection);

        StartCoroutine(HarpoonFireProcess());
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    /// <returns> The delay till the next iteration </returns>
    private IEnumerator HarpoonFireProcess()
    {
        CheckAimAtBoat();
        float travelDistance = 0f;
        while (travelDistance < HarpoonGun.Instance.GetHarpoonMaxDistance())
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = transform.forward * HarpoonGun.Instance.GetHarpoonProjectileSpeed() * Time.deltaTime;

            // If no collision, move the harpoon
            HarpoonFiredProjectileMovement(movement);
            travelDistance += movement.magnitude;

            yield return null;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame
            if (Physics.Raycast(transform.position, movement, out RaycastHit hit,
                movement.magnitude, ~HarpoonGun.Instance.GetHarpoonExcludeLayers()))
            {
                transform.position = hit.point; // Snap the harpoon to the _hit point
                break;
            }
        }
        //Either reached here because we hit something or because we have exceeded the max distance
        //If the harpoon sticks in the object it remains enabled. Otherwise it disables it
        gameObject.SetActive(HarpoonGun.Instance.GetDoesHarpoonRemainsInObject());
    }

    /// <summary>
    /// Moves the harpoon when its being fired out
    /// </summary>
    /// <param name="movement"> The change in position of the harpoon </param>
    private void HarpoonFiredProjectileMovement(Vector3 movement)
    {
        transform.position += movement;
    }

    /// <summary>
    /// Handles the process for whether the player is aiming at the boat (or anything on it)
    /// If the player is aiming at these objects, it will child the harpoon to the Moving Objects parent object
    /// </summary>
    private void CheckAimAtBoat()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit) && RecursiveCheckForParent(hit.collider.transform,_movingObjects))
        {
            transform.parent = _movingObjects;
        }
    }

    /// <summary>
    /// Recursively checks if the child is a child, grandchild, etc. of the parent
    /// </summary>
    /// <param name="child">lowest object we are looking for in the heirarchy</param>
    /// <param name="parent">Object we hope to find as the parent</param>
    /// <returns>True if child is within parent in the hierarchy</returns>
    private bool RecursiveCheckForParent(Transform child, Transform parent)
    {
        if (child.parent == null)
        {
            return false;
        }
        if (child.parent == parent)
        {
            return true;
        }
        return RecursiveCheckForParent(child.parent, parent);

    }
}