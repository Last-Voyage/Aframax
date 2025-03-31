/*****************************************************************************
// File Name :         DamageVeinPulse.cs
// Author :            Adam Garwacki
// Creation Date :     3/29/25
//
// Brief Description : Makes the opacity of the on-screen veins portraying
                       damage taken waver occasionally.
// Notes :             Uses code from HeartAppearance() in PlayerHealthUI.cs.
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lets visual damage states flicker or pulse while onscreen.
/// </summary>
public class DamageVeinPulse : MonoBehaviour
{   
    [SerializeField] private float _delayBetweenPulses;
    [SerializeField] private float _pulsePersistTime;
    [SerializeField] private float _timeTakenToPulse;
    [SerializeField] private float _minOpacity;
    
    private float _maxOpacity;
    private float _alphaValueRatio;

    private Image _imageComponent;

    private Color _baseColor;

    private WaitForSeconds _pulseDelayWait;
    private WaitForSeconds _pulsePersistWait;

    /// <summary>
    /// Initializes the alpha of the GameObject while setting a reference to
    /// this GameObject's Image component, and begins the pulse coroutine.
    /// </summary>
    private void OnEnable()
    {
        _imageComponent = gameObject.GetComponent<Image>();
        _maxOpacity = _imageComponent.color.a;

        _baseColor = _imageComponent.color;
        _imageComponent.color = new(_baseColor.r, _baseColor.g, _baseColor.b, _maxOpacity);

        _pulseDelayWait = new(_delayBetweenPulses);
        _pulsePersistWait = new(_pulsePersistTime);

        StartCoroutine(VeinPulse());
    }

    /// <summary>
    /// Makes the damage effect pulse on occasion. Perpetually loops while active.
    /// </summary>
    /// <returns></returns>
    private IEnumerator VeinPulse()
    {
        while(true)
        {
            // Lerp to completely visible
            float appearTime = 0;

            while (appearTime < _timeTakenToPulse)
            {
                appearTime += Time.deltaTime;

                // Get the ratio of time, lerp the alpha
                _alphaValueRatio = appearTime / _timeTakenToPulse;
                _imageComponent.color = new(1, 1, 1, Mathf.Lerp(_maxOpacity, _minOpacity, _alphaValueRatio));

                yield return null;
            }

            // Safeguard: Make sure that it's at minimum opacity at the end
            _imageComponent.color = new(1, 1, 1, _minOpacity);

            // Keep the heart on screen for some time
            yield return _pulsePersistWait;

            // Lerp to completely hidden
            appearTime = 0;

            while (appearTime < _timeTakenToPulse)
            {
                appearTime += Time.deltaTime;

                // Get the ratio of time, lerp the alpha
                _alphaValueRatio = appearTime / _timeTakenToPulse;
                _imageComponent.color = new(_baseColor.r, _baseColor.g, _baseColor.b, 
                    Mathf.Lerp(_minOpacity, _maxOpacity, _alphaValueRatio));

                yield return null;
            }

            // Safeguard: Make sure that it's at full opacity at the end
            _imageComponent.color = new(_baseColor.r, _baseColor.g, _baseColor.b, _maxOpacity);

            yield return _pulseDelayWait;
        }
        
    }

    /// <summary>
    /// Stops the coroutine that makes the damage effect pulse.
    /// </summary>
    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
