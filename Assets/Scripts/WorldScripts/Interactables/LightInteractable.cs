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
    private float _lengthOfMaxFlickering;
    [SerializeField]
    private float _lengthOfMinFlickering;

    [SerializeField]
    private float _maxIntensity = 10f;

    [SerializeField] 
    private float _minIntensity = .1f;
    
    [SerializeField]
    [Tooltip("Time it takes after the flickering changes for the light to turn off")]
    private float _lastTimeBeforeTurningOff = 1f;

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
        float randomTimeOn = Random.Range(_lengthOfMinFlickering, _lengthOfMaxFlickering);

        _realLightBulb.intensity = Random.Range(_minIntensity,_maxIntensity);
        
        // get max time and min time
        
        for (float i = 0; i < randomTimeOn-_lastTimeBeforeTurningOff; i++)
        {
            yield return new WaitForSeconds(1f);
            _realLightBulb.intensity = Random.Range(_minIntensity,_maxIntensity);
        }
        
        // Wait for 1 second at the end, then turn off the light
        
        yield return _waitBeforeTurnOff;

        _causeLightFlicker = null;

        _realLightBulb.intensity = 0f;
    }
}
