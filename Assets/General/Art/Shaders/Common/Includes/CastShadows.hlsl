/**************************************************************************
// File Name :          CastShadows.hlsl
// Author :             Miles Rogers
// Creation Date :      9/18/2024
//
// Brief Description :  Gathers all shadow data from the Unity World.
**************************************************************************/

#ifndef CAST_SHADOWS_INCLUDED
#define CAST_SHADOWS_INCLUDED

void CalculateCustomShadows_float(
    float3 WorldPos,
    out float DistanceAtten,
    out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    // No need for this information in the
    // ShaderGraph preview
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    // Gather shadow coordinate info from 
    // screen-space, if it exists
    float4 clipPos = TransformWorldToHClip(WorldPos);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    // Gather world-space shadow coordinate info
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    // Sample shadow info from main light
    // (DistanceAtten * ShadowAtten = Full Shadow Data)
    Light mainLight = GetMainLight(shadowCoord);
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

#endif