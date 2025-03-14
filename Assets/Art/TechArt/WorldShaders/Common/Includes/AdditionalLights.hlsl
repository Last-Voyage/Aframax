/**************************************************************************
// File Name :          AdditionalLights.hlsl
// Author :             Miles Rogers
// Creation Date :      9/18/2024
//
// Brief Description :  Gathers all non-main directional lighting info
//                      from the Unity world.
**************************************************************************/

#ifndef ADDITIONAL_LIGHTS_INCLUDED
#define ADDITIONAL_LIGHTS_INCLUDED

float3 CalculateAdditionalLights_float(
    float3 Position,
    float3 Normal,
    out float3 Color)
{
    Color = float3(0.0F, 0.0F, 0.0F);

#ifndef SHADERGRAPH_PREVIEW
    // (No need to sample this in the ShaderGraph preview)
    
#ifdef _ADDITIONAL_LIGHTS
    // Find the number of additional lights in the scene
    uint numAdditionalLights = GetAdditionalLightsCount();

    // Get the data from each light, add to color output
    for (uint i = 0; i < numAdditionalLights; i++)
    {
        Light light = GetAdditionalLight(
            i, Position
        );

        float3 radiance = light.color * 
            light.shadowAttenuation *
            light.distanceAttenuation;

        Color += (dot(Normal, light.direction) * radiance);
    }
#endif
#endif

    return Color;
}

#endif