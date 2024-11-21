/*****************************************************************************
// File Name :         HarpoonProjectileMovement.cs
// Author :            Ryan Swanson
// Contributors:       David Henvick
// Creation Date :     10/28/2024
//
// Brief Description : Controls the movement of the harpoon projectile
*****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls the movement of the harpoon projectile fired from the gun
/// </summary>
public class HarpoonProjectileMovement : MonoBehaviour
{
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
}