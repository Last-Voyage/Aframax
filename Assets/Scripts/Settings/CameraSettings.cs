/*****************************************************************************
// File Name :         CamSettings.cs
// Author :            Nabil Tagba
// Creation Date :     3/5/2025
//
// Brief Description : Gets the sensitivity settings and applys it to the camera
*****************************************************************************/
using Cinemachine;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Gets the sensitivity settings and applys it to the camera
/// </summary>
public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string _gameplaySettingFilePath;

    public static Action WasSettingsChanged;

    /// <summary>
    /// happens when script instance is loaded
    /// subscibe to was settings changed action
    /// </summary>
    private void Awake()
    {
        WasSettingsChanged += UpdateSettings;
    }

    /// <summary>
    /// happens when the game starts
    /// </summary>
    private void Start()
    {
        WasSettingsChanged.Invoke();
    }

    /// <summary>
    /// Gets the sensitivity settings and applys it to the camera
    /// </summary>
    private void UpdateSettings()
    {
        float sensitivity = SaveManager.Instance.GetGameSaveData().CameraSensitivty;

        CinemachinePOV cinemachinePOV = GetComponent<CinemachineVirtualCamera>().
            GetCinemachineComponent<CinemachinePOV>();

        //sensitivity
        if (PlayerCameraController.Instance != null)
        {
            PlayerCameraController.Instance.StoredSensitivity = new Vector2(sensitivity,sensitivity);

            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = sensitivity;
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = sensitivity;
        }

        cinemachinePOV.m_HorizontalAxis.m_InvertInput = 
            SaveManager.Instance.GetGameSaveData().IsCameraXAxisInverted;

        cinemachinePOV.m_VerticalAxis.m_InvertInput =
            !SaveManager.Instance.GetGameSaveData().IsCameraYAxisInverted;
    }

    /// <summary>
    /// called when the script is destroyed
    /// unsubscribe to was settings changed action
    /// </summary>
    private void OnDestroy()
    {
        WasSettingsChanged -= UpdateSettings;
    }
}
