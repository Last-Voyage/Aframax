/*****************************************************************************
// File Name :         CamSettings.cs
// Author :            Nabil Tagba
// Creation Date :     3/5/2025
//
// Brief Description : Gets the sensitivity settings and applys it to the camera
*****************************************************************************/
using Cinemachine;
using System.IO;
using UnityEngine;

/// <summary>
/// Gets the sensitivity settings and applys it to the camera
/// </summary>
public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string _gameplaySettingFilePath;
    public bool WasSettingsChanged = true;

    /// <summary>
    /// happens when the game starts
    /// </summary>
    private void Start()
    {
        UpdateSettings();
    }

    /// <summary>
    /// happens at a fixed frame rate
    /// </summary>
    private void FixedUpdate()
    {
        //only happens when the settings are changed
        if (WasSettingsChanged)
        {
            UpdateSettings();
            WasSettingsChanged = false;
        }
        
    }

    /// <summary>
    /// Gets the sensitivity settings and applys it to the camera
    /// </summary>
    private void UpdateSettings()
    {
        
        string[] camSettings = File.ReadAllLines(Application.streamingAssetsPath +
            _gameplaySettingFilePath)[0].Split(" ");
        print(camSettings);

        for (int i = 0; i < camSettings.Length; i++)
        {
            switch (i)
            {
                case 0:
                    //sensitivity
                    GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>()
                        .m_VerticalAxis.m_MaxSpeed = float.Parse(camSettings[0]);
                    GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>()
                        .m_HorizontalAxis.m_MaxSpeed = float.Parse(camSettings[0]);
                    break;
                case 1:
                    //invert X
                    GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>()
                        .m_HorizontalAxis.m_InvertInput = bool.Parse(camSettings[1]);
                    break;
                case 2:
                    //invert Y
                    GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>()
                        .m_VerticalAxis.m_InvertInput = !bool.Parse(camSettings[2]);
                    break;

                default:
                    break;
            }
        }

    }
}
