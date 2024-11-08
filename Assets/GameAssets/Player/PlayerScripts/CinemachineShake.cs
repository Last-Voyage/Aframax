/*****************************************************************************
// File Name :         CinemachineShake.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/2024
//
// Brief Description : Shakes the camera ;)
*****************************************************************************/

using UnityEngine;
using Cinemachine;

/// <summary>
/// Controls shakes to the cinemachine
/// </summary>
public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    private CinemachineVirtualCamera _cinemachineVirtualCam;
    private float _shakeTimer;

    /// <summary>
    /// creates an instance of the CinemachineShake object so you can call a shake anywhere you want.
    /// </summary>
    private void Awake() 
    {
        Instance = this;
        _cinemachineVirtualCam = GetComponent<CinemachineVirtualCamera>();
    }
    
    /// <summary>
    /// times the shake after its is called from where ever it is called. When time is up stop the shake
    /// </summary>
    /// <param name="intensity"> how intense the shake is </param>
    /// <param name="time"> duration of the shake </param>
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;
    }

    /// <summary>
    /// times the shake
    /// </summary>
    private void Update()
    {
        if (!(_shakeTimer > 0)) return;
        
        _shakeTimer -= Time.deltaTime;
        
        if (!(_shakeTimer <= 0f)) return;
        
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //turns off the shake
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
    }
}
