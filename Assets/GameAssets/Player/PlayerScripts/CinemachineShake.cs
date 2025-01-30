/*****************************************************************************
// File Name :         CinemachineShake.cs
// Author :            Tommy Roberts
// Contributors:       Andrew Stapay
// Creation Date :     9/22/2024
//
// Brief Description : Shakes the camera ;)
*****************************************************************************/

using UnityEngine;
using Cinemachine;
using System.Collections;

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
    /// <param name="decreasingIntensity"> true if the screen shake should slowly decrease over time, 
    /// false otherwise </param>
    public void ShakeCamera(float intensity, float time, bool decreasingIntensity)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;

        StartCoroutine(ResolveShaking(decreasingIntensity));
    }

    /// <summary>
    /// Resolves the screen shake based on input variables
    /// </summary>
    /// <param name="decreasingIntensity"> true if the screen shake should slowly decrease over time, 
    /// false otherwise </param>
    /// <returns></returns>
    private IEnumerator ResolveShaking(bool decreasingIntensity)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        while (_shakeTimer > 0)
        {
            // Decrease our timer
            _shakeTimer -= Time.deltaTime;

            // If we are decreasing the intensity, do that too now
            if (decreasingIntensity)
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                    Mathf.Lerp(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain, 0, Time.deltaTime);
            }

            yield return null;
        }

        //turns off the shake
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
    }
}
