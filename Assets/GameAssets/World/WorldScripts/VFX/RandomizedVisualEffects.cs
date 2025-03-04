/******************************************************************************
// File Name:       RandomizedVisualEffects.cs
// Author:          Ryan Swanson
// Creation Date:   March 3rd, 2025
//
// Description:     Allows for vfx to be randomly played
******************************************************************************/

using PrimeTween;
using UnityEngine;

/// <summary>
/// Plays visual effects on random intervals
/// </summary>
public class RandomizedVisualEffects : MonoBehaviour
{
    [Tooltip("The minimum amount of time for it to work before plays the next vfx")]
    [SerializeField] private float _minVfxDelay;
    [Tooltip("The maximum amount of time for it to work before plays the next vfx")]
    [SerializeField] private float _maxVfxDelay;

    private Tween _delayTween;

    private ParticleSystem _associatedParticleSystem;

    /// <summary>
    /// Sets associated variables and starts the delay for the first time
    /// </summary>
    void Start()
    {
        _associatedParticleSystem = GetComponent<ParticleSystem>();
        StartRandomDelay();
    }

    /// <summary>
    /// Starts the random delay before playing the vfx
    /// </summary>
    private void StartRandomDelay()
    {
        if (!gameObject.activeInHierarchy) return;

        _delayTween = Tween.Delay(this, Random.Range(_minVfxDelay, _maxVfxDelay), PlayVisualEffect);
    }

    /// <summary>
    /// Plays the visual effect and resets the delay
    /// </summary>
    protected virtual void PlayVisualEffect()
    {
        _associatedParticleSystem.Play();
        StartRandomDelay();
    }

    /// <summary>
    /// Stop the tween if it is active while this is destroyed
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_delayTween.isAlive)
        {
            _delayTween.Stop();
        }
    }
}
