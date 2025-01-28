/*****************************************************************************
// File Name :         LightInteractable.cs
// Author :            Nick Rice
// Creation Date :     1/28/25
//
// Brief Description : This is a light switch that can be interacted with
*****************************************************************************/
using System.Collections;
using UnityEngine;

public class LightInteractable : MonoBehaviour, IPlayerInteractable
{
    [SerializeField]
    private Light realLightBulb;

    private float baseIntensity = 10f;

    private IEnumerator causeLightFlicker;

    private WaitForSeconds lastSecondBeforeTurnOff = new WaitForSeconds(1f);

    /// <summary>
    /// Checks if the light is currently on; if it is, it turns off
    /// Otherwise, it turns on the light
    /// </summary>
    public void OnInteractedByPlayer()
    {
        if (causeLightFlicker != null)
        {
            realLightBulb.intensity = 0f;
            StopCoroutine(causeLightFlicker);
            causeLightFlicker = null;
        }
        else
        {
            causeLightFlicker = Flickering();
            StartCoroutine(causeLightFlicker);
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
            realLightBulb.intensity = Random.Range(.1f,baseIntensity);
            yield return new WaitForSeconds(i/2f);

            realLightBulb.intensity = baseIntensity;
            yield return new WaitForSeconds(i/2f);
        }
        
        // Wait for 1 second at the end, then turn off the light
        
        yield return lastSecondBeforeTurnOff;

        causeLightFlicker = null;

        realLightBulb.intensity = 0f;
    }
}
