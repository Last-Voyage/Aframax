/**************************************************************************
// File Name :          GlobalIllumination.hlsl
// Author :             Miles Rogers
// Creation Date :      9/18/2024
//
// Brief Description :  Gathers all Global Illumination data from the
//                      Unity world.
**************************************************************************/

#ifndef GI_INCLUDED
#define GI_INCLUDED

void CalculateGlobalIllumination_float(
    float3 Position,
    float3 Normal,
    float2 LightmapUV,
    out float3 Color)
{
    float4 shadowCoord;
    float3 bakedGI;

#ifdef SHADERGRAPH_PREVIEW
    // No need for this information in the
    // ShaderGraph preview
    shadowCoord = 0.0F;
    bakedGI = 0.0F;
#else
    // Sample the lightmap UVs
    float2 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);

    // Sample the current vertex's spherical
    // harmonics data in the Unity scene
    float3 vertexSH;
    OUTPUT_SH(Normal, vertexSH);

    // Sample the baked Global Illumination
    bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);

    // Combine the baked GI with the realtime GI
    Light mainLight = GetMainLight(shadowCoord, Position, 1);
    MixRealtimeAndBakedGI(mainLight, Normal, bakedGI);
#endif
    
    Color = bakedGI;
}

#endif