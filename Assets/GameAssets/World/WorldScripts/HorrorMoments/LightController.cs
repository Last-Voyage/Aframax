/**********************************************************************************************************************
// File Name :          LightController.cs
// Author :             Andrew Stapay
// Creation Date :      1/31/25
//
// Brief description :  Controls various aspects of the lighting, such as the Light Shift Horror Moment
**********************************************************************************************************************/
using System.Collections;
using UnityEngine;

/// <summary>
/// Controls various aspects of the lights
/// </summary>
public class LightController : MonoBehaviour
{
    // Components of the GameObject
    private Light _light;
    private Color _originalColor;
    private Animator _animator;

    // Light Shift Variables
    [SerializeField] private Color _lightShiftTargetColor = new Color(0, 0.396f, 0.114f, 0);
    [SerializeField] private float _lightShiftDuration = 20f;

    /// <summary>
    /// Called on the first frame
    /// Used to set up variables
    /// </summary>
    private void Awake()
    {
        GetLight();
        GetAnimator();
    }

    /// <summary>
    /// Gets the light component and related variables
    /// </summary>
    private void GetLight()
    {
        _light = GetComponent<Light>();
        _originalColor = _light.color;
    }

    /// <summary>
    /// Gets the animator component
    /// </summary>
    private void GetAnimator()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Performs the Light Shift Horror Moment
    /// </summary>
    private void LightShift()
    {
        // Shift the color
        StartCoroutine(ShiftColor());
    }

    /// <summary>
    /// Keeps track of the light animation and changes the color as needed
    /// </summary>
    private IEnumerator ShiftColor()
    {
        // Change the lights to target color
        float timer = 0;
        while (timer <= 1)
        {
            float newR = Mathf.Lerp(_light.color.r, _lightShiftTargetColor.r, timer);
            float newG = Mathf.Lerp(_light.color.g, _lightShiftTargetColor.g, timer);
            float newB = Mathf.Lerp(_light.color.b, _lightShiftTargetColor.b, timer);

            _light.color = new Color(newR, newG, newB, 0);
            timer += Time.deltaTime;

            yield return null;
        }

        // Now, we need the lights to stay this color until the set time has elapsed
        timer = _lightShiftDuration;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Finally, we can change the lights back to normal
        timer = 0;
        while (timer <= 1)
        {
            float newR = Mathf.Lerp(_light.color.r, _originalColor.r, timer);
            float newG = Mathf.Lerp(_light.color.g, _originalColor.g, timer);
            float newB = Mathf.Lerp(_light.color.b, _originalColor.b, timer);

            _light.color = new Color(newR, newG, newB, 0);
            timer += Time.deltaTime;

            yield return null;
        }

        // Confirm we made it back to the original color
        _light.color = _originalColor;
    }

    private void LightFlicker()
    {
        _animator.SetTrigger("PlayFlicker");
    }

    /// <summary>
    /// Called when the GameObject is enabled
    /// Used to subscribe to events
    /// </summary>
    private void OnEnable()
    {
        VfxManager.Instance.GetOnLightShiftEvent().AddListener(LightShift);
        VfxManager.Instance.GetOnLightFlickerEvent().AddListener(LightFlicker);
    }

    /// <summary>
    /// Called when the GameObject is disabled
    /// Used to unsubscribe to events
    /// </summary>
    private void OnDisable()
    {
        VfxManager.Instance.GetOnLightShiftEvent().RemoveListener(LightShift);
        VfxManager.Instance.GetOnLightFlickerEvent().RemoveListener(LightFlicker);
    }
}
