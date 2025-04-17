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
    /// Reference to the current fake window
    /// </summary>
    private FakeWindowObject _fakeWindow;

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
    /// Sets the current fake window
    /// </summary>
    /// <param name="fakeWindow">The fake window reference</param>
    public void SetCurrentFakeWindow(FakeWindowObject fakeWindow)
    {
        _fakeWindow = fakeWindow;
        _captureContainerCamera.transform.position = _initialCameraPosition;
    }

    /// <summary>
    /// Find the closest window object at a given frame, get
    /// vector between camera and that window, set capture
    /// camera position/rotation accordingly.
    /// </summary>
    void Update()
    {
        // Do nothing if fake window is not set
        if (_fakeWindow.IsUnityNull())
        {
            return;
        }
        
        // Get global camera position
        var cameraPos = transform.position;
        
        // Get rotation based on the vector between the closest
        // window and the current camera position
        var newRotation = Quaternion.LookRotation(
            _fakeWindow.transform.position - cameraPos
        ).eulerAngles;

        // Account for level/window rotation
        newRotation.y -= _fakeWindow.transform.eulerAngles.y - 90.0F;
        
        // Prevent any weird gimbal stuff when converting from
        // quaternion -> euler rotation
        newRotation.z = 0.0F;

        // Set capture camera rotation
        _captureContainerCamera.transform.eulerAngles = 
            newRotation;
        
        // Get offset to be applied to the capture camera
        var offset = _fakeWindow.transform.position - cameraPos;

        // Clamp follow distance
        offset.x = Mathf.Clamp(offset.x, -_maxFollowDistance, _maxFollowDistance);
        offset.y = 0.0F; // Never adjust y-position
        offset.z = Mathf.Clamp(offset.z, -_maxFollowDistance, _maxFollowDistance);

        // Set capture camera position
        _captureContainerCamera.transform.position =
            _initialCameraPosition + offset;
    }
}
