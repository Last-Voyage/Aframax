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
    private Vector3 _movement;

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
            if (_isStuck)
            {
                break;
            }
            
            // Calculate how far the harpoon should move in this frame
            _movement = transform.forward * HarpoonGun.Instance.GetHarpoonProjectileSpeed() * Time.deltaTime;

            // If no collision, move the harpoon
            HarpoonFiredProjectileMovement(_movement);
            travelDistance += _movement.magnitude;
            
            yield return null;
        }
        //Either reached here because we hit something or because we have exceeded the max distance
        //If the harpoon sticks in the object it remains enabled. Otherwise it disables it
        gameObject.SetActive(HarpoonGun.Instance.GetDoesHarpoonRemainsInObject());
    }

    /// <summary>
    /// Moves the harpoon when its being fired out
    /// </summary>
    /// <param name="movement"></param>
    private void HarpoonFiredProjectileMovement(Vector3 movement)
    {
        transform.position += movement;
    }

    /// <summary>
    /// Used to find out if the harpoon hits something or not. 
    /// </summary>
    /// <returns></returns> a bool as to whether or not the while loop should be broken
    public bool ShouldHarpoonStick(GameObject objectToStickIn)
    {
        //checks if _stuck is true, if it is it ends the while loop
        if (!HarpoonGun.Instance.GetDoesHarpoonRemainsInObject())
        {
            return false;
        }

        gameObject.transform.parent = objectToStickIn.transform;
        _isStuck = true;

        return _isStuck;
    }
}