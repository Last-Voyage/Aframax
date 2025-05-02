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

    /// <summary>
    /// How far from the initial position the camera is allowed to be adjusted
    /// (For the parallax effect)
    /// </summary>
    [SerializeField] private float _maxFollowDistance = 10.0F;

    /// <summary>
    /// List of the fake windows currently in the scene
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

        _fakeWindows = new List<FakeWindowObject>();

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
    /// Adds a fake window to the global registry
    /// </summary>
    /// <param name="fakeWindow">The fake window reference</param>
    public void RegisterWindow(FakeWindowObject fakeWindow)
    {
        _fakeWindows.Add(fakeWindow);
    }

    /// <summary>
    /// Removes a fake window from the global registry
    /// </summary>
    /// <param name="fakeWindow">The fake window reference</param>
    public void UnregisterWindow(FakeWindowObject fakeWindow)
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
        // Do nothing if no fake windows are registered
        if (_fakeWindows.Count == 0)
        {
            return;
        }
        
        // Get global camera position
        var cameraPos = transform.position;
        
        // Find nearest window
        float minDist = float.MaxValue;
        var closestWindow = _fakeWindows[0];

        // For every window in the registry
        foreach (var window in _fakeWindows)
        {
            // Do not consider if window is not in view
            if (!window.WindowMeshRenderer.isVisible)
            {
                continue;
            }
            
            float curDist = Vector3.Distance(
                cameraPos,
                window.transform.position
            );

            // Check if window is closer than any previously
            // evaluated windows
            if (curDist < minDist)
            {
                minDist = curDist;
                closestWindow = window;
            }
        }
        
        // Do nothing if fake window is not set
        if (closestWindow.IsUnityNull())
        {
            return;
        }
        
        // Get rotation based on the vector between the closest
        // window and the current camera position
        var newRotation = Quaternion.LookRotation(
            closestWindow.transform.position - cameraPos
        ).eulerAngles;

        // Account for level/window rotation
        newRotation.y -= closestWindow.transform.eulerAngles.y - 90.0F;
        
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
