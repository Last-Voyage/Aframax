/**************************************************************************
// File Name :          GaussianBlur.hlsl
// Author :             Miles Rogers
// Creation Date :      3/2/2024
//
// Brief Description :  Applies a gaussian blur based on an intensity value
//
//                      (Samples are hardcoded to 5 for the sake of 
//                      simplicity, we should not need more)
**************************************************************************/

#ifndef GAUSSIAN_BLUR_INCLUDED
#define GAUSSIAN_BLUR_INCLUDED

void GaussianBlur_float(
    float2 UV, 
    float2 TexelSize, 
    float BlurAmount, 
    UnityTexture2D MainTex, 
    UnitySamplerState State,
    out float4 Color)
{
    /* ===== CONSTRUCT BLUR KERNEL ==== */

    float2 offsets[5] = 
    { 
        float2(-2.0, 0.0), 
        float2(-1.0, 0.0), 
        float2( 0.0, 0.0), 
        float2( 1.0, 0.0), 
        float2( 2.0, 0.0) 
    };

    /* ===== DEFINE GAUSSIAN WEIGHTS ==== */

    float weights[5] = 
    { 
        0.06136, 
        0.24477, 
        0.38774, 
        0.24477, 
        0.06136 
    };

    /* ===== CONVOLVE NEIGHBORING PIXELS ==== */

    Color = float4(0, 0, 0, 0);
    
    for (int i = 0; i < 5; i++)
    {
        for (int j = 0; j < 5; j++)
        {
            float2 sampleUV = UV + float2(offsets[i].x, offsets[j].y) 
                * TexelSize * BlurAmount;

            Color += weights[i] * weights[j] * SAMPLE_TEXTURE2D(
                MainTex, 
                State, 
                sampleUV
            );
        }
    }
}

#endif