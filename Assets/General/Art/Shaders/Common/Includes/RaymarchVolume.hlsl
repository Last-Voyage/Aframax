/**************************************************************************
// File Name :          CastShadows.hlsl
// Author :             Miles Rogers
// Creation Date :      9/18/2024
//
// Brief Description :  Gathers all shadow data from the Unity World.
**************************************************************************/

#ifndef RAYMARCH_VOLUME_INCLUDED
#define RAYMARCH_VOLUME_INCLUDED

void RaymarchVolume_float(
    UnityTexture3D VolumeTex,
    UnitySamplerState VolumeSampler,
    float3 ObjectOrigin,
    float3 RayOrigin,
    float3 RayDirection,
    float DensityValue,
    float DensityScale,
    float DistanceFalloff,
    float NumSteps,
    float StepSize,
    float3 Offset,
    out float Result)
{
    float density = 0;

    for(int i = 0; i < NumSteps; i++)
    {
        RayOrigin += RayDirection * StepSize;

        // Calculate density
        density += DensityValue * 
            (distance(RayOrigin, ObjectOrigin) * DistanceFalloff);
    }

    Result = density * DensityScale;
}

#endif