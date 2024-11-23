/******************************************************************************
// File Name:       GeneralVfxFunctionality.cs
// Author:          Ryan Swanson
//Contributor:  Mark Hanson
// Creation Date:   October 22nd, 2024
//
// Description:     Handles any general functionality related to an object with particle systems
******************************************************************************/

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Provides the general functionality for all vfx
/// </summary>
public class GeneralVfxFunctionality : MonoBehaviour
{
    [Tooltip("All vfx to play when activated")]
    private ParticleSystem[] _particleSystems;
    [Tooltip("All animations to play with an associated vfx if it has any")]
    private Animator[] _vfxAnimators;

    [Tooltip("The animation trigger used for all vfx animations")]
    private const string VFX_ANIMATION_TRIGGER = "PlayVfxAnim";
    
    [Tooltip("The trigger used for if vfx should move")]
    [SerializeField] private bool _isMoveableVfx = false;
    [Tooltip("Speed of how fast the vfx moves (Number is used for division)")]
    [SerializeField] private float _vfxSpeed = 2f;
    
    /// <summary>
    /// Adds all particle systems attached to this to a list
    /// </summary>
    public void SetupChildParticleSystems()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _vfxAnimators = GetComponentsInChildren<Animator>();
    }

    /// <summary>
    /// Plays all vfx on this object
    /// </summary>
    public void StartAllVfx()
    {
        //Play all vfx
        foreach (ParticleSystem particleSystem in _particleSystems)
        {
            particleSystem.Play();
        }
        //Play all animations
        foreach (Animator animator in _vfxAnimators)
        {
            animator.SetTrigger(VFX_ANIMATION_TRIGGER);
        }
    }
/// <summary>
/// To move Vfx forward if need be
/// </summary>
    private void Update()
    {
        if (_isMoveableVfx)
        {
            transform.position += ((transform.forward / _vfxSpeed) * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Gets the longest duration of all particle systems this object has
    /// </summary>
    /// <returns></returns>
    public float GetLongestParticleSystemDuration()
    {
        float currentLongestDuration = 0;

        //Iterate through all particle systems
        foreach (ParticleSystem particleSystem in _particleSystems)
        {
            //Get the combined duration and lifetime
            float tempDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constant;

            //Check if that is longer than all previous particle systems
            if(tempDuration > currentLongestDuration)
            {
                currentLongestDuration = tempDuration;
            }
        }

        return currentLongestDuration;
    }
}
