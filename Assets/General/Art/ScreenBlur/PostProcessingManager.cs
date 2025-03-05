/**************************************************************************
// File Name :          PostProcessingManager.cs
// Author :             Caelie Joyner
// Contributer :        Charlie Polonus
// Creation Date :      3/3/2025
//
// Brief Description :  Adjusts the blur intensity of the post-processing
                        Gaussian blur
**************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Manages the post-processing effects
/// </summary>
public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] [Range(0,1)] private float _blurPercentage;
    private Coroutine _blurCoroutine = null;

#if UNITY_EDITOR

    /// <summary>
    /// Allows people in the unity editor to test out certain blur values
    /// </summary>
    private void FixedUpdate()
    {
        // Make sure the blur isn't being changed by the game already
        if (_blurCoroutine == null)
        {
            SetCameraBlur(_blurPercentage);
        }
    }

#endif

    /// <summary>
    /// Sets the blur of the camera to a certain value over time
    /// </summary>
    /// <param name="endBlurValue">The end value to be set to</param>
    /// <param name="time">The time taken to get to the end value</param>
    public void BlurCamera(float endBlurValue, float time)
    {
        // Stop all other attempts to change the blur, and start the coroutine
        StopAllCoroutines();
        _blurCoroutine = StartCoroutine(ChangeCameraBlurOverTime(endBlurValue, time));
    }

    /// <summary>
    /// A BlurCamera override for use in the StoryManager
    /// </summary>
    /// <param name="time">The time taken to get to max blur</param>
    public void MaxBlurOverTime(float time)
    {
        BlurCamera(1, time);
    }

    /// <summary>
    /// A BlurCamera override for use in the StoryManager
    /// </summary>
    /// <param name="time">The time taken to get to no blur</param>
    public void RemoveBlurOverTime(float time)
    {
        BlurCamera(0, time);
    }

    /// <summary>
    /// The coroutine in charge of blurring the camera
    /// </summary>
    /// <param name="endBlur">The end value to be set to</param>
    /// <param name="time">The time taken to get to the end value</param>
    /// <returns>Whatever an IEnumerator is idk</returns>
    private IEnumerator ChangeCameraBlurOverTime(float endBlur, float time)
    {
        float startBlur = _blurPercentage;

        // Starts a timer
        float curTime = 0;
        while (curTime < time)
        {
            // Proceeds through time, find the ratio of "finished" curTime is
            curTime += Time.deltaTime;
            float timeRatio = curTime / time;

            // Blurs the camera according to the start and end blurs, and the time needed
            _blurPercentage = Mathf.Lerp(startBlur, endBlur, timeRatio);
            SetCameraBlur(_blurPercentage);
            yield return null;
        }
        // Sets it to the final value and removes itself from the coroutine
        SetCameraBlur(endBlur);
        _blurCoroutine = null;
    }

    /// <summary>
    /// Sets the camera blur to certain values based on the percentage
    /// </summary>
    /// <param name="blurValue"></param>
    private void SetCameraBlur(float blurValue)
    {
        // Clamps the blur percentage
        _blurPercentage = Mathf.Clamp01(blurValue);

        // Finds the depth of field effect and sets the variables
        _postProcessingVolume.sharedProfile.TryGet(out DepthOfField depthOfField);
        depthOfField.gaussianStart.value = Mathf.Lerp(1000, 0, _blurPercentage);
        depthOfField.gaussianMaxRadius.value = Mathf.Lerp(1, 1.5f, _blurPercentage);
    }

    // Remove all blur settings when the game is closed
    private void OnApplicationQuit()
    {
        SetCameraBlur(0);
    }
}
