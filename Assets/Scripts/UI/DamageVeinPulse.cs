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

public class DamageVeinPulse : MonoBehaviour
{   
    [SerializeField] private float _delayBetweenPulses;
    [SerializeField] private float _timeTakenToPulse;
    [SerializeField] private float _pulsePersistTime;
    [SerializeField] private float _minOpacity;
    
    private float _maxOpacity;
    private Image _imageComponent;

    private bool _canPulse = true;

    /// <summary>
    /// Initializes the alpha of the GameObject while setting a reference to
    /// this GameObject's Image component, and begins the pulse coroutine.
    /// </summary>
    private void OnEnable()
    {
        _imageComponent = gameObject.GetComponent<Image>();
        _maxOpacity = _imageComponent.color.a;

        _imageComponent.color = new(1, 1, 1, _maxOpacity);

        StartCoroutine(VeinPulse());
    }

    /// <summary>
    /// Makes the damage effect pulse on occassion.
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
                float alphaValueRatio = appearTime / _timeTakenToPulse;
                _imageComponent.color = new(1, 1, 1, Mathf.Lerp(_maxOpacity, _minOpacity, alphaValueRatio));

                yield return null;
            }
            // Safeguard: Make sure that it's at minimum opacity at the end
            _imageComponent.color = new(1, 1, 1, _minOpacity);

            // Keep the heart on screen for some time
            yield return new WaitForSeconds(_pulsePersistTime);

            // Lerp to completely hidden
            appearTime = 0;
            while (appearTime < _timeTakenToPulse)
            {
                appearTime += Time.deltaTime;

                // Get the ratio of time, lerp the alpha
                float alphaValueRatio = appearTime / _timeTakenToPulse;
                _imageComponent.color = new(1, 1, 1, Mathf.Lerp(_minOpacity, _maxOpacity, alphaValueRatio));

                yield return null;
            }
            // Safeguard: Make sure that it's at full opacity at the end
            _imageComponent.color = new(1, 1, 1, _maxOpacity);

            yield return new WaitForSeconds(_delayBetweenPulses);
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
