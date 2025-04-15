/**********************************************************************************************************************
// File Name :          FakeWindowManager.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Global singleton that creates the fake window capture container in the current scene
**********************************************************************************************************************/

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
    /// Register global singleton, spawn capture container if it does not exist already
    /// </summary>
    private void Awake()
    {
        // Global static registry
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        Instantiate(
            _fakeWindowCaptureContainer,
            _worldSpawnPos,
            Quaternion.identity
        );
    }
}
