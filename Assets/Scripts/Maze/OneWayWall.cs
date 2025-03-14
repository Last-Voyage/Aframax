using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayWall : MonoBehaviour
{
    private Transform _mainCamera;

    private BoxCollider _boxCollider;
    private MeshRenderer _meshRenderer;
    private float _internalClock = 0.0F;
    
    static readonly float CheckStateInterval = 0.5F;
    
    private void Start()
    {
        _mainCamera = Camera.main.transform;

        _boxCollider = GetComponent<BoxCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Using coroutine because we don't need to c
    private void Update()
    {
        _internalClock += Time.deltaTime;

        if (_internalClock > CheckStateInterval)
        {
            CheckState();
        }
    }

    private void CheckState()
    {
        var cameraAlignedPos = _mainCamera.localToWorldMatrix.MultiplyPoint(
            transform.position
        );
        var shouldEnable = (_mainCamera.position - cameraAlignedPos).x > 0.0F;

        if (_boxCollider.enabled == shouldEnable ||
            _meshRenderer.enabled == shouldEnable)
        {
            return;
        }
        
        _boxCollider.enabled = shouldEnable;
        _meshRenderer.enabled = shouldEnable;
    }
}
