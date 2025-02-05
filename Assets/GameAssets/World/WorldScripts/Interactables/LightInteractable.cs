/*****************************************************************************
// File Name :         LightInteractable.cs
// Author :            Nick Rice
// Creation Date :     1/28/25
//
// Brief Description : This is a light switch that can be interacted with
*****************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// This is a light switch that can be interacted with
/// </summary>
public class LightInteractable : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private Light _realLightBulb;

    [SerializeField] 
    private float _lastTimeBeforeTurningOff = 1f;

    [SerializeField]
    private float _baseIntensity = 10f;
    [SerializeField] 
    private float _lengthOfFlickering;

    private IEnumerator _causeLightFlicker;

    private WaitForSeconds _waitBeforeTurnOff;

    private void Start()
    {
        _waitBeforeTurnOff = new WaitForSeconds(_lastTimeBeforeTurningOff);
    }

    /// <summary>
    /// Checks if the light is currently on; if it is, it turns off
    /// Otherwise, it turns on the light
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if (_causeLightFlicker != null)
        {
            _realLightBulb.intensity = 0f;
            StopCoroutine(_causeLightFlicker);
            _causeLightFlicker = null;
        }
        else
        {
            _causeLightFlicker = Flickering();
            StartCoroutine(_causeLightFlicker);
        }
    }

    /// <summary>
    /// Light flickering
    /// </summary>
    private IEnumerator Flickering()
    {
        float randomTimeOn = Random.Range(2f, 7f);

        for (float i = 0; i < randomTimeOn-1f; i += Random.Range(randomTimeOn/5f, randomTimeOn/2f))
        {
            _realLightBulb.intensity = Random.Range(.1f,_baseIntensity);
            yield return new WaitForSeconds(i/2f);

            _realLightBulb.intensity = _baseIntensity;
            yield return new WaitForSeconds(i/2f);
        }
        
        // Wait for 1 second at the end, then turn off the light
        
        yield return _waitBeforeTurnOff;

        _causeLightFlicker = null;

        _realLightBulb.intensity = 0f;
    }
}
