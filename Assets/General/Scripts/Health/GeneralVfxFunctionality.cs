/******************************************************************************
// File Name:       GeneralVfxFunctionality.cs
// Author:          Ryan Swanson
// Creation Date:   October 22nd, 2024
//
// Description:     Handles any general functionality related to an object with particle systems
******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the general functionality for all vfx
/// </summary>
public class GeneralVfxFunctionality : MonoBehaviour
{
    private List<ParticleSystem> _particleSystems = new();

    /// <summary>
    /// Performs all needed setup
    /// </summary>
    public void Setup()
    {
        SetupChildParticleSystems();
    }

    /// <summary>
    /// Adds all particle systems attached to this to a list
    /// </summary>
    private void SetupChildParticleSystems()
    {
        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            _particleSystems.Add(particleSystem);
        }
    }

    /// <summary>
    /// Plays all vfx on this object
    /// </summary>
    public void StartAllVfx()
    {
        foreach (ParticleSystem particleSystem in _particleSystems)
        {
            particleSystem.Play();
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
        foreach (ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
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
