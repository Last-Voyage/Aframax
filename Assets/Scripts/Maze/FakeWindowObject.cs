using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWindowObject : MonoBehaviour
{
    private FakeWindowManager _fakeWindowManager;
    private Transform _cameraTransform;

    [SerializeField] private MeshRenderer _windowMeshRenderer;
    
    private static readonly int _CAMERA_POS = 
        Shader.PropertyToID("_CameraPos");
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
