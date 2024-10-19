using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogVolume : MonoBehaviour
{
    public float Radius = 100.0F;

    private void Start()
    {
        VolumetricFogPass.AddFogVolume(this);
    }

    private void OnDestroy()
    {
        VolumetricFogPass.RemoveFogVolume(this);
    }

    public void Apply(MaterialPropertyBlock propertyBlock)
    {
        // Eventually we will use this to apply all the material properties
    }
}
