/*****************************************************************************
// File Name :         DeathScreenFadeIn.cs
// Author :            Jeremiah Peters
// Creation Date :     9/28/24
//
// Brief Description : makes the death screen UI fade in when the scene is loaded
*****************************************************************************/
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// does the fading in of the death screen ui elements
/// </summary>
public class DeathScreenFadeIn : MonoBehaviour
{
    private CanvasGroup _deathScreenUiElements;

    [Tooltip("rough time it takes for things to fade all the way in")]
    [SerializeField] private float _fadeTime;

    [Tooltip("time before the fade in starts")]
    [SerializeField] private float _fadeDelay;

    [Tooltip("threshold for the speed up to start (should be between 0 and 1")]
    [SerializeField] private float _fadeSpeedUpThreshold;

    [Tooltip("how much faster it gets when the threshold is reached (very small number like 1.25)")]
    [SerializeField] private float _fadeSpeedUpMultiplier;

    private void Awake()
    {
        _deathScreenUiElements = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _deathScreenUiElements.alpha = 0;
        StartCoroutine(UiFadeIn(_deathScreenUiElements));
    }

    /// <summary>
    /// Handles increasing the alpha of a canvas group from 0 to 1 gradually.
    /// with some tweaking this could be used by other things but for now it is private
    /// could maybe be repurposed as a manager or something later if need be
    /// </summary>
    /// <param name="objectsToFadeIn"></param>
    /// <returns></returns>
    private IEnumerator UiFadeIn(CanvasGroup objectsToFadeIn)
    {
        yield return new WaitForSeconds(_fadeDelay);
        float elapsedtime = 0;

        while (_deathScreenUiElements.alpha < 1)
        {
            elapsedtime += Time.deltaTime;

            //fade slightly faster over half way through
            //I think this looks slightly better but it can be changed
            if (objectsToFadeIn.alpha > _fadeSpeedUpThreshold)
            {
                objectsToFadeIn.alpha = Mathf.Lerp(0, 1, elapsedtime / _fadeTime * _fadeSpeedUpMultiplier);
            }
            else
            {
                objectsToFadeIn.alpha = Mathf.Lerp(0, 1, elapsedtime / _fadeTime);
            }

            yield return null;
        }
    }

}
