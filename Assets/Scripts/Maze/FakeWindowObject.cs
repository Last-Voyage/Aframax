/**********************************************************************************************************************
// File Name :          FakeWindowObject.cs
// Author :             Miles Rogers
// Creation Date :      4/15/2025
//
// Brief description :  Attach this script to objects that need to render fake windows. It ensures the existence
//                      of the FakeWindowManager context and updates the relevant values in the window mesh's
//                      material.
**********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Script for objects that need to render fake windows. Ensures the existence of the FakeWindowManager
/// context and updates the relevant values in the window mesh's material.
/// </summary>
public class FakeWindowObject : MonoBehaviour
{
    /// <summary>
    /// Reference to the FakeWindowManager global singleton
    /// </summary>
    private FakeWindowManager _fakeWindowManager;
    
    /// <summary>
    /// Reference to the player camera's transform value
    /// </summary>
    private Transform _cameraTransform;

    /// <summary>
    /// The mesh renderer rendering the fake window texture (assumes the correct
    /// fake window material is assigned)
    /// </summary>
    [SerializeField] private MeshRenderer _windowMeshRenderer;
    
    /// <summary>
    /// Camera position cached shader property
    /// </summary>
    private static readonly int _CAMERA_POS = 
        Shader.PropertyToID("_CameraPos");
    
    /// <summary>
    /// Position of this object cached shader property
    /// </summary>
    private static readonly int _OBJ_POS = 
        Shader.PropertyToID("_ObjPos");

    /// <summary>
    /// Ensure existence of FakeWindowManager, which spawns
    /// the capture container
    /// </summary>
    private void Start()
    {
        _fakeWindowManager = FakeWindowManager.Instance;
        _cameraTransform = _fakeWindowManager.transform;
    }

    /// <summary>
    /// Update values in fake window material instance on the window mesh
    /// renderer.
    /// </summary>
    private void Update()
    {
        _windowMeshRenderer.material.SetVector(
            _CAMERA_POS,
            _cameraTransform.position
        );
        _windowMeshRenderer.material.SetVector(
            _OBJ_POS,
            transform.position
        );
    }
}
