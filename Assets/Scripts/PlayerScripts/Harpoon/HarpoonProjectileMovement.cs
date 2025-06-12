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

    private Transform _movingObjects;
    private Camera _mainCamera;

    /// <summary>
    /// Instantiates _movingObjects 
    /// </summary>
    private void Awake()
    {
        _movingObjects = FindObjectOfType<BoatMover>()?.gameObject.transform;
        _mainCamera = Camera.main;
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

        IsHit = false;

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
        // Prevent the harpoon from impaling into an object while already impaled
        if (IsHit)
        {
            return;
        }
        
        transform.position = Physics.ClosestPoint(transform.position, 
            other.GetComponent<Collider>(),other.transform.position,other.transform.rotation);
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
            if (block.CompareTag("Enemy") || block.CompareTag("Environment"))
            {
                IsHit = true;
            }
        }
    }

    /// <summary>
    /// Handles the process for whether the player is aiming at the boat (or anything on it)
    /// If the player is aiming at these objects, it will child the harpoon to the Moving Objects parent object
    /// </summary>
    private void CheckAimAtBoat()
    {
        _mainCamera = Camera.main;
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
        if (child.parent.IsUnityNull())
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