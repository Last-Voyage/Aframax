/*****************************************************************************
// File Name :         CinemachineShake.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Shakes the camera ;)
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    private CinemachineVirtualCamera _cinemachineVirtualCam;
    private float _shakeTimer;
    /// <summary>
    /// creates an instance of the CinemachineShake object so you can call a shake anywhere you wwant.
    /// </summary>
    private void Awake() {
        Instance = this;
        _cinemachineVirtualCam = GetComponent<CinemachineVirtualCamera>();
    }
    /// <summary>
    /// times the shake
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void ShakeCamera(float intensity, float time){
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;
    }
    /// <summary>
    /// times the shake
    /// </summary>
    private void Update() {
        if(_shakeTimer > 0){
            _shakeTimer -= Time.deltaTime;
            if(_shakeTimer <= 0f){
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                    _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
