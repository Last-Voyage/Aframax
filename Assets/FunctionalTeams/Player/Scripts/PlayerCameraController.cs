/******************************************************************************
// File Name:       PlayerCameraController.cs
// Author:          Andrew Stapay
// Creation Date:   September 19, 2024
//
// Description:     Implementation of the basic camera control for a player 
//                  character. This script takes input from the mouse and
//                  allows the Main Camera to rotate in the scene.
******************************************************************************/
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CinemachineVirtualCamera))]

public class PlayerCameraController : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;

    private Coroutine _cameraCoroutine;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized
    /// and to start the coroutine
    /// </summary>
    void Start()
    {
        InitializeCamera();

        _cameraCoroutine = StartCoroutine("MoveCamera");
    }

    private void InitializeCamera()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// Camera coroutine
    /// This will perpetually call the camera moving method until disabled
    /// </summary>
    private IEnumerator MoveCamera()
    {
        while (true)
        {
            yield return null;
        }
    }
}
