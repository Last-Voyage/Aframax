/**************************************************************************
// File Name :          GaussianBlur.hlsl
// Author :             Miles Rogers
// Creation Date :      3/3/2024
//
// Brief Description :  Applies a gaussian blur based on an intensity value
//
//                      The BlurAmount value increases the standard deviation
//                      a.k.a. how blurry the final image is.
//
//                      (Changing the SAMPLE_RADIUS definition will increase
//                      the amount of pixels walked per-fragment [more
//                      SAMPLE_RADIUS == more possible blur])
**************************************************************************/

#ifndef GAUSSIAN_BLUR_INCLUDED
#define GAUSSIAN_BLUR_INCLUDED

#define SAMPLE_RADIUS 10

float CalculateGaussianWeight(float2 xy, float blurIntensity)
{
    return exp(-(xy.x * xy.x + xy.y * xy.y) / 
        (2.0 * blurIntensity * blurIntensity) /
        (2.0 * blurIntensity * blurIntensity)
    );
}

void GaussianBlur_float(
    float2 UV, 
    float2 TexelSize, 
    float BlurAmount, 
    UnityTexture2D MainTex, 
    UnitySamplerState State,
    out float4 Color)
{
    // Do not apply effect if BlurAmount is 0
    if (BlurAmount < 0.01)
    {
        Color = float4(SAMPLE_TEXTURE2D(
            MainTex, 
            State, 
            UV
        ).xyz, 1.0);
        return;
    }

    int2 imageDimensions = float2(1.0, 1.0) / TexelSize;
    float2 pixelCoord = UV * imageDimensions;

    int N = SAMPLE_RADIUS * 2 + 1;

    float3 sum = float3(0.0, 0.0, 0.0);
    float weightSum = 0.0;

    // Convolve neighboring pixels
    for (int i = 0; i < N; i++)
    {
        for (int j = 0; j < N; j++)
        {
            float2 offset = float2(
                SAMPLE_RADIUS - i,
                SAMPLE_RADIUS - j
            );
            float2 sampleCoord = UV + (offset / imageDimensions);

            float weight = CalculateGaussianWeight(offset, BlurAmount);

            sum += SAMPLE_TEXTURE2D(
                MainTex, 
                State, 
                sampleCoord
            ) * float3(weight, weight, weight);
            weightSum += weight;
        }
    }

    // Divide by total to get the mean color (final blurred fragment)
    Color = float4(
        sum / float3(weightSum, weightSum, weightSum), 
        1.0
    );
}

#endif