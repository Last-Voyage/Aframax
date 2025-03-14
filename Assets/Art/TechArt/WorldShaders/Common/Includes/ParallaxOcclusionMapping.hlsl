/**************************************************************************
// File Name :          ParallaxOcclusion Mapping.hlsl
// Author :             Miles Rogers
// Creation Date :      10/16/2024
//
// Brief Description :  Custom implementation of Parallax Occlusion
//                      mapping, a technique needed to boost material
//                      depth with a height map
**************************************************************************/

#ifndef PARALLAX_OCCLUSION_INCLUDED
#define PARALLAX_OCCLUSION_INCLUDED

#pragma exclude_renderers d3d11_9x
#pragma exclude_renderers d3d9

void ParallaxOcclusionMapping_float(
    UnityTexture2D HeightmapTexture,
    float HeightScale,
    float MinLayers,
    float MaxLayers,
    float3 ViewDirection,
    float2 TexCoords,
    out float2 PomUvs)
{
#ifdef SHADERGRAPH_PREVIEW
    HeightScale = 0.05F;
    MinLayers = 8.0F;
    MaxLayers = 64.0F;
    ViewDirection = float3(0.0F, 0.25F, 0.0F);
    TexCoords = 0.0F;
#endif

    float numLayers = lerp(
        MaxLayers,
        MinLayers,
        abs(dot(float3(0.0F, 0.0F, 1.0F), ViewDirection))
    );

    float layerDepth = 1.0F / numLayers;
    float currentLayerDepth = 0.0F;
    
    float2 raymarchVector = ViewDirection.xy / ViewDirection.z * HeightScale;
    float2 deltaUvs = raymarchVector / numLayers;
    
    float2 uvs = TexCoords;
    float currentDepthMapValue = 1.0F - tex2D(HeightmapTexture, uvs).r;
    
    // Loop until we find an intersection on the height map
    UNITY_LOOP
    while(currentLayerDepth < currentDepthMapValue)
    {
        uvs -= deltaUvs;
        currentDepthMapValue = 1.0F - tex2D(HeightmapTexture, uvs);
        currentLayerDepth += layerDepth;
    }
    
    // Apply occlusion
    float2 prevTexCoords = uvs + deltaUvs;
    
    float afterDepth = currentDepthMapValue - currentLayerDepth;
    float beforeDepth = 1.0F - tex2D(HeightmapTexture, prevTexCoords).r - 
        currentLayerDepth + layerDepth;
    float weight = afterDepth / (afterDepth - beforeDepth);
    
    uvs = prevTexCoords * weight + uvs * (1.0F - weight);
    
    // Discard all fragments outside of texture
    if (uvs.x > 1.0F || uvs.y > 1.0F || 
        uvs.x < 0.0F || uvs.y < 0.0F)
    {
        return;
    }
    
    PomUvs = uvs;
}

#endif