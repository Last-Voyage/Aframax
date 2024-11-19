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
using UnityEngine;

/// <summary>
/// Controls the movement of the harpoon projectile fired from the gun
/// </summary>
public class HarpoonProjectileMovement : MonoBehaviour
{
    //movement is a vector3 used for the harpoon's current velocity
    static private Vector3 movement;

    private bool _isStuck;
    
    /// <summary>
    /// Fires the harpoon and sets the position and rotation. Is called by the harpoon gun
    /// </summary>
    /// <param name="startLoc"></param>
    /// <param name="startDirection"></param>
    public void LaunchHarpoon(Vector3 startLoc, Vector3 startDirection)
    {
        transform.position = startLoc;
        transform.LookAt(transform.position + startDirection);

        StartCoroutine(HarpoonFireProcess());
        
        _isStuck = false;
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    private IEnumerator HarpoonFireProcess()
    {
        float travelDistance = 0f;
        while (travelDistance < HarpoonGun.Instance.GetHarpoonMaxDistance())
        {
            // Calculate how far the harpoon should move in this frame
            movement = transform.forward * HarpoonGun.Instance.GetHarpoonProjectileSpeed() * Time.deltaTime;

            // If no collision, move the harpoon
            HarpoonFiredProjectileMovement(movement);
            travelDistance += movement.magnitude;

            StickHarpoon();
            
            yield return null;
        }
        //Either reached here because we hit something or because we have exceeded the max distance
        //If the harpoon sticks in the object it remains enabled. Otherwise it disables it
        //gameObject.SetActive(HarpoonGun.Instance.GetDoesHarpoonRemainsInObject());
    }

    /// <summary>
    /// Moves the harpoon when its being fired out
    /// </summary>
    /// <param name="movement"></param>
    private void HarpoonFiredProjectileMovement(Vector3 movement)
    {
        transform.position += movement;
    }

    private void StickHarpoon()
    {
        if (_isStuck)
        {
            return;
        }
            
        // Cast a ray from the harpoon's current position forward by the amount it moves this frame
        if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit,
            movement.magnitude, ~HarpoonGun.Instance.GetHarpoonExcludeLayers()))
        {
            Debug.Log(hit.collider.gameObject.name);
            Debug.Log(hit.point);
            gameObject.transform.parent = hit.collider.transform;
            transform.position = hit.point; // Snap the harpoon to the _hit point
            _isStuck = true;
        }
        //If the harpoon sticks in the object it remains enabled. Otherwise it disables it
        gameObject.SetActive(HarpoonGun.Instance.GetDoesHarpoonRemainsInObject());
    }

    /// <summary>
    /// Used to destroy the harpoon projectile when it hits something in scene.
    /// </summary>
    /// <param name="block"></param> what it collides against. 
    private void OnTriggerEnter(Collider block)
    {
        if (!block.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth unneeded))
        {
            //StickHarpoon();
        }
    }
}