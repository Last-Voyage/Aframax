/**********************************************************************************************************************
// File Name :          LightController.cs
// Author :             Andrew Stapay
// Creation Date :      1/31/25
//
// Brief description :  Controls various aspects of the lighting, such as the Light Shift Horror Moment
**********************************************************************************************************************/
using System.Collections;
using System.Xml;
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

    [Header("Flicker")]
    // Toggle for turning on and off light flickering
    [SerializeField] private bool _canLightFlicker;
    [SerializeField] private float _lightFlickerDuration;
    [SerializeField] private float _horrorFlickerDuration;
    [SerializeField] private AnimationCurve _flickerCurve;
    [SerializeField] private AnimationCurve _horrorMomentCurve;
    private float _startingIntensity;

    [Header("Light Shift")]
    // Light Shift Variables
    [SerializeField] private Color _lightShiftTargetColor = new Color(0, 0.396f, 0.114f, 0);
    [SerializeField] private bool _doesReturnColor;
    [SerializeField] private float _lightTransitionTime = 1f;
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
        _startingIntensity = _light.intensity;
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
            _light.color = Color.Lerp(_originalColor, _lightShiftTargetColor, timer);

            timer += Time.deltaTime / _lightTransitionTime;

            yield return null;
        }

        // Confirm we made it to the target color
        _light.color = _lightShiftTargetColor;

        // If we don't want the lights to change back, we can skip the rest of this
        if (_doesReturnColor)
        {
            // Now, we need the lights to stay this color until the set time has elapsed
            yield return new WaitForSeconds(_lightShiftDuration);

            // Finally, we can change the lights back to normal
            timer = 0;
            while (timer <= 1)
            {
                _light.color = Color.Lerp(_lightShiftTargetColor, _originalColor, timer);

                timer += Time.deltaTime / _lightTransitionTime;

                yield return null;
            }

            // Confirm we made it back to the original color
            _light.color = _originalColor;
        }
    }

    /// <summary>
    /// Begins the light flickering animation by setting the trigger
    /// </summary>
    private void LightFlicker()
    {
        if (_canLightFlicker)
        {
            StartLightFlickerProcess();
        }
    }

    /// <summary>
    /// Starts the light flicker process
    /// </summary>
    private void StartLightFlickerProcess()
    {
        StartCoroutine(LightFlickerProcess());
    }

    /// <summary>
    /// The process of the light flickering
    /// </summary>
    /// <returns>Time itself</returns>
    private IEnumerator LightFlickerProcess()
    {
        float flickerTimer = 0;
        float flickerDuration = _canLightFlicker ? _horrorFlickerDuration: _lightFlickerDuration;
        AnimationCurve curve = _canLightFlicker ? _horrorMomentCurve: _flickerCurve;

        while (flickerTimer < 1)
        {
            flickerTimer += Time.deltaTime / _lightFlickerDuration;
            _light.intensity = curve.Evaluate(flickerTimer) * _startingIntensity;
            yield return null;
        }
    }

    /// <summary>
    /// Called when the GameObject is enabled
    /// Used to subscribe to events
    /// </summary>
    private void OnEnable()
    {
        VfxManager.Instance.GetOnLightShiftEvent().AddListener(LightShift);
        VfxManager.Instance.GetOnLightFlickerEvent().AddListener(LightFlicker);

        if (TryGetComponent<RandomizedAnimation>(out RandomizedAnimation anim))
        {
            anim.OnAnimPlay.AddListener(StartLightFlickerProcess);
        }
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
