using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVfxFunctionality : MonoBehaviour
{
    private List<ParticleSystem> _particleSystems = new();

    public void Setup()
    {
        foreach(ParticleSystem particleSystem in GetComponentsInChildren<ParticleSystem>())
        {
            _particleSystems.Add(particleSystem);
        }
    }

    public void StartAllVfx()
    {
        foreach (ParticleSystem particleSystem in _particleSystems)
        {
            particleSystem.Play();
        }
    }
}
