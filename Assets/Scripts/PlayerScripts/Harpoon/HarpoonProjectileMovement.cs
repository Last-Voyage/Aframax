/*****************************************************************************
// File Name :         HarpoonProjectileMovement.cs
// Author :            Ryan Swanson
// Contributors:       David Henvick, Alex Kalscheur, Jeremiah Peters
// Creation Date :     10/28/2024
//
// Brief Description : Controls the movement of the harpoon projectile
*****************************************************************************/

using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Controls the movement of the harpoon projectile fired from the gun
/// </summary>
public class HarpoonProjectileMovement : MonoBehaviour
{
    [Tooltip("How far the harpoon moves into the object when impaled")]
    [SerializeField] private float _harpoonImpactDepth;
    //for hitting walls
    internal bool IsHit { get; set; }
    
    private HarpoonDamage _harpoonDamage;

    /// <summary>
    /// Fires the harpoon and sets the position and rotation. Is called by the harpoon gun
    /// </summary>
    /// <param name="startLocation"> The location that the harpoon begins at </param>
    /// <param name="startDirection"> The direction the harpoon is fired in </param>
    public void LaunchHarpoon(Vector3 startLocation, Vector3 startDirection)
    {
        if (_harpoonDamage.IsUnityNull())
        {
            _harpoonDamage = GetComponent<HarpoonDamage>();
        }
        
        transform.position = startLocation;
        transform.LookAt(transform.position + startDirection);

        IsHit = false;
        _harpoonDamage.CanApplyDamage = true;

        StartCoroutine(HarpoonFireProcess());
    }

    /// <summary>
    /// coroutine to move the created harpoon to the target direction. starts the reel coroutine at the end
    /// </summary>
    /// <returns> The delay till the next iteration </returns>
    private IEnumerator HarpoonFireProcess()
    {
        float travelDistance = 0f;
        while (travelDistance < HarpoonGun.Instance.GetHarpoonMaxDistance() && !IsHit)
        {
            // Calculate how far the harpoon should move in this frame
            Vector3 movement = transform.forward * (HarpoonGun.Instance.GetHarpoonProjectileSpeed() * Time.deltaTime);

            // If no collision, move the harpoon
            HarpoonFiredProjectileMovement(movement);
            travelDistance += movement.magnitude;

            yield return null;

            // Cast a ray from the harpoon's current position forward by the amount it moves this frame
            if (Physics.Raycast(transform.position, movement, out RaycastHit hit,
                movement.magnitude, ~HarpoonGun.Instance.GetHarpoonExcludeLayers()))
            {
                ImpaleHarpoon(hit);
                IsHit = true;
                break;
            }
        }
        //Either reached here because we hit something or because we have exceeded the max distance
        //If the harpoon sticks in the object it remains enabled. Otherwise it disables it
        gameObject.SetActive(HarpoonGun.Instance.GetDoesHarpoonRemainsInObject());
    }

    /// <summary>
    /// Impales the harpoon into a surface based on a raycast
    /// </summary>
    /// <param name="hit"> The raycast to use </param>
    public void ImpaleHarpoon(RaycastHit hit)
    {
        // Prevent the harpoon from impaling into an object while already impaled
        if (IsHit)
        {
            return;
        }

        _harpoonDamage.CanApplyDamage = false;
        transform.position = hit.point; // Snap the harpoon to the _hit point
        // Move forward to adjust for impact depth
        transform.position += transform.forward * _harpoonImpactDepth;
    }

    /// <summary>
    /// Impales the harpoon into a surface based on a collider
    /// </summary>
    /// <param name="other"> The collider we contacted</param>
    public void ImpaleHarpoon(Collider other)
    {
        //child the harpoon to whatever it hit
        //now if that object moves, the harpoon will move with it
        if (other.gameObject.CompareTag("HarpoonStickObject"))
        {
            // Only sticks into objects with the correct tag
            // This is done so we don't get oddities in which the harpoon sticks into a wall,
            //  sets itself as a child, and inherits a potential unusual scale
            gameObject.transform.parent = other.gameObject.transform;
        }
        
        // Prevent the harpoon from impaling into an object while already impaled
        if (IsHit)
        {
            return;
        }
        
        _harpoonDamage.CanApplyDamage = false;
        
        // Finds the closest point on the collider to stick to
        transform.position = Physics.ClosestPoint(transform.position, 
            other,other.transform.position,other.transform.rotation);
        // Move forward to adjust for impact depth
        transform.position += transform.forward * _harpoonImpactDepth;
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
    /// On trigger Enter, activated once harpoon hits something. used to stop harpoon when it hits walls
    /// </summary>
    /// <param name="block"></param> the collider
    private void OnTriggerEnter(Collider block)
    {
        if(!block.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth unneeded))
        {
            if (block.CompareTag("Environment"))
            {
                IsHit = true;
            }
        }
    }
}