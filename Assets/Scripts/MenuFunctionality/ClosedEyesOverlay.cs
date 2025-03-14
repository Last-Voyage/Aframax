/******************************************************************************
// File Name:       ClosedEyesOverlay.cs
// Author:          Charlie Polonus
// Creation Date:   February 4, 2025
//
// Description:     Implementation of the eyes closing "camera disabling"
//                  visual effect
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The MonoBehaviour Managing the turning on and off of the camera with a fading effect
/// </summary>
public class ClosedEyesOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _overlayImage;

    [Header("Settings")]
    [SerializeField] private bool _isOpenOnStart;
    private float _opacityValue;

    /// <summary>
    /// Set the color of the eyes to open or closed based on whether it should be open on start
    /// </summary>
    void Start()
    {
        _overlayImage.color = _isOpenOnStart ? Color.clear : Color.black;
        _opacityValue = _isOpenOnStart ? 0 : 1;
    }

    /// <summary>
    /// Fade the overlay to black
    /// </summary>
    /// <param name="time">The time taken to fade to black</param>
    public void FadeToBlack(float time)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToValue(1, time));
    }

    /// <summary>
    /// Fade the overlay to clear
    /// </summary>
    /// <param name="time">The time taken to fade to clear</param>
    public void FadeToClear(float time)
    {
        StopAllCoroutines();
        StartCoroutine(FadeToValue(0, time));
    }

    /// <summary>
    /// Coroutine to fade the overlay to a certain value
    /// </summary>
    /// <param name="endValue">The ending value to be lerped to</param>
    /// <param name="time">The total time taken</param>
    /// <returns></returns>
    private IEnumerator FadeToValue(float endValue, float time)
    {
        // Set the start value
        float startValue = _opacityValue;

        // Begin the timer
        float curTime = 0;
        while (curTime < time)
        {
            // Iterate the time and update the color value
            curTime += Time.deltaTime;
            float timeRatio = Mathf.Clamp01(curTime / time);
            _opacityValue = Mathf.Lerp(startValue, endValue, timeRatio);

            // Set the color to the lerped value
            _overlayImage.color = Color.Lerp(Color.clear, Color.black, _opacityValue);
            yield return null;
        }

        // Set the final color to the final value
        _opacityValue = endValue;
        _overlayImage.color = Color.Lerp(Color.clear, Color.black, _opacityValue);
    }
}
