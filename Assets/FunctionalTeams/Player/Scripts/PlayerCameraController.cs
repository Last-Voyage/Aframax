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
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]

/// <summary>
/// A class that controls the camera attached to the player.
/// Can be expanded upon to do whatever we need with the camera.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    // Variable for the Virtual Camera
    // Unused at the moment, but it'll be here when we eventually need it
    private CinemachineVirtualCamera _virtualCamera;

    // Variables that relate to the camera's coroutine
    private Coroutine _cameraCoroutine;

    /// <summary>
    /// This function is called before the first frame update.
    /// Used to initialize any variables that are not serialized
    /// and to start the coroutine
    /// </summary>
    void Start()
    {
        // Get the Virtual Camera component and start the coroutine
        InitializeCamera();

        _cameraCoroutine = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// Initialize the camera variable.
    /// </summary>
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
            AdjustPlayerRotation();
            yield return null;
        }
    }

    /// <summary>
    /// Adjusts the player model such that it faces the same way as the camera.
    /// Used to assist movement.
    /// </summary>
    private void AdjustPlayerRotation()
    {
        // Cinemachine actually manipulates the Main Camera itself
        // By getting the rotation of the Main Camera, we can rotate our character
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    /// <summary>
    /// Activates or deactivates the camera coroutine based on the input boolean
    /// Used when the OnCameraMovementToggled Action is invoked
    /// </summary>
    /// <param name="change"> Determines if the camera movement should be turned on or off </param>
    private void ToggleCameraMovement(bool change)
    {
        if (change)
        {
            _cameraCoroutine = StartCoroutine(MoveCamera());
        }
        else
        {
            StopCoroutine(_cameraCoroutine);
        }
    }

    /// <summary>
    /// Called when this component is enabled.
    /// Used to assign the OnCameraMovementToggled Action to a listener
    /// </summary>
    private void OnEnable()
    {
        CameraManager.Instance.GetCameraMovementToggleEvent().AddListener(ToggleCameraMovement);
    }

    /// <summary>
    /// Called when this component is disnabled.
    /// Used to unassign the OnCameraMovementToggled Action to a listener
    /// </summary>
    private void OnDisable()
    {
        CameraManager.Instance.GetCameraMovementToggleEvent().RemoveListener(ToggleCameraMovement);
    }
}
