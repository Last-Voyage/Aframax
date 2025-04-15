/**********************************************************************************************************************
// File Name :          FakeWindowManager.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Global singleton that creates the fake window capture container in the current scene
**********************************************************************************************************************/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Global singleton that creates the fake window capture container in the current scene
/// </summary>
[RequireComponent(typeof(Camera))]
public class FakeWindowManager : MonoBehaviour
{
    /// <summary>
    /// Global singleton instance
    /// </summary>
    public static FakeWindowManager Instance { get; private set; }
    
    /// <summary>
    /// Prefab for the fake window capture container
    /// </summary>
    [SerializeField] private GameObject _fakeWindowCaptureContainer;
    
    /// <summary>
    /// Spawn position for the window capture container in the world
    /// </summary>
    [SerializeField] private Vector3 _worldSpawnPos = new(0.0F, 0.0F, 1500.0F);

    [SerializeField] private float _maxFollowDistance = 10.0F;
    
    /// <summary>
    /// Reference to every fake window in the scene
    /// </summary>
    private List<FakeWindowObject> _fakeWindows;

    /// <summary>
    /// The camera responsible for rendering the fake windows
    /// </summary>
    private Camera _captureContainerCamera;

    /// <summary>
    /// _captureContainerCamera's initial position in WS
    /// </summary>
    private Vector3 _initialCameraPosition; 
    
    /// <summary>
    /// Register global singleton, spawn capture container if it does not exist already
    /// </summary>
    private void Awake()
    {
        // Global static registry
        if (!Instance.IsUnityNull() && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        // Allocate fake window list
        _fakeWindows = new();

        // Spawn capture container in scene
        var container = Instantiate(
            _fakeWindowCaptureContainer,
            _worldSpawnPos,
            Quaternion.identity
        );
        
        // Grab camera from capture container
        _captureContainerCamera = container.GetComponentInChildren<Camera>();

        // Ensure check was successful
        if (_captureContainerCamera.IsUnityNull())
        {
            throw new NullReferenceException(
                "Unable to resolve capture container camera!"
            );
        }

        // Record initial position
        _initialCameraPosition = _captureContainerCamera.transform.position;
    }

    /// <summary>
    /// Add a fake window object to the global registry
    /// </summary>
    /// <param name="fakeWindow">The fake window reference</param>
    public void RegisterFakeWindow(FakeWindowObject fakeWindow)
    {
        _fakeWindows.Add(fakeWindow);
    }

    /// <summary>
    /// Remove a fake window object to the global registry
    /// </summary>
    /// <param name="fakeWindow">The fake window reference</param>
    public void UnregisterFakeWindow(FakeWindowObject fakeWindow)
    {
        _fakeWindows.Remove(fakeWindow);
    }

    /// <summary>
    /// Find the closest window object at a given frame, get
    /// vector between camera and that window, set capture
    /// camera position/rotation accordingly.
    /// </summary>
    void Update()
    {
        // Find the closest window (going with a naive brute-force
        // search because checking distance is pretty quick and
        // there will probably not be any more than 2 or 3 windows
        // in a given scene)
        FakeWindowObject closestWindow = null;
        Vector3 cameraPos = transform.position;

        float dist = 0.0F;
        float minDistance = float.MaxValue;

        foreach (var window in _fakeWindows)
        {
            // Discard null entries
            if (window.IsUnityNull())
            {
                continue;
            }
            
            dist = Vector3.Distance(cameraPos, window.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closestWindow = window;
            }
        }

        // If there is no result, no need to run anything else
        if (closestWindow.IsUnityNull())
        {
            return;
        }

        // Get rotation based on the vector between the closest
        // window and the current camera position
        var newRotation = Quaternion.LookRotation(
            closestWindow.transform.position - cameraPos
        ).eulerAngles;
        
        // Prevent any weird gimbal stuff when converting from
        // quaternion -> euler rotation
        newRotation.z = 0.0F;

        // Set capture camera rotation
        _captureContainerCamera.transform.eulerAngles = 
            newRotation;
        
        // Get offset to be applied to the capture camera
        var offset = closestWindow.transform.position - cameraPos;

        // Clamp follow distance
        offset.x = Mathf.Clamp(offset.x, -_maxFollowDistance, _maxFollowDistance);
        offset.y = 0.0F; // Never adjust y-position
        offset.z = Mathf.Clamp(offset.z, -_maxFollowDistance, _maxFollowDistance);

        // Set capture camera position
        _captureContainerCamera.transform.position =
            _initialCameraPosition + offset;
    }
}
