using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpectrumParameters
{
    public float scale;
    public float angle;
    public float spreadBlend;
    public float swell;
    public float alpha;
    public float peakOmega;
    public float gamma;
    public float shortWavesFade;
};

[System.Serializable]
public struct DisplaySpectrumSettings
{
    [Range(0, 1)]
    public float scale;
    public float windSpeed;
    public float windDirection;
    public float fetch;
    [Range(0, 1)]
    public float spreadBlend;
    [Range(0, 1)]
    public float swell;
    public float peakEnhancement;
    public float shortWavesFade;
}

public class CalculateConjugatedSpectrum : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Texture noiseTexture;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RenderTexture wavesDataTexture;
    [SerializeField] private int resolution = 256;

    private static int _resultID = Shader.PropertyToID("Result");
    private static int _waveDataTexID = Shader.PropertyToID("WavesData");
    private static int _noiseID = Shader.PropertyToID("GaussianNoise");
    private static int _spectrumsID = Shader.PropertyToID("Spectrums");
    private static int _cutoffLowID = Shader.PropertyToID("CutoffLow");
    private static int _cutoffHighID = Shader.PropertyToID("CutoffHigh");
    private static int _gravityID = Shader.PropertyToID("GravityAcceleration");
    private static int _lengthScaleID = Shader.PropertyToID("LengthScale");
    private static int _depthID = Shader.PropertyToID("Depth");
    

    [SerializeField] private DisplaySpectrumSettings spectrumSettings;

    private int _kernelID = 0;
    
    [SerializeField] private float lowCutoff = 0.0F;
    [SerializeField] private float highCutoff = 1000.0F;
    [SerializeField] private float gravity = 9.81F;
    [SerializeField] private float lengthScale = 1.0F;
    [SerializeField] private float depth = 1000.0F;

    private ComputeBuffer _paramsBuffer;

    private void Start()
    {
        // Fill SpectrumParameters struct
        SpectrumParameters sParams;
        sParams.scale = spectrumSettings.scale;
        sParams.angle = spectrumSettings.windDirection / 
            180.0F * Mathf.PI;
        sParams.spreadBlend = spectrumSettings.spreadBlend;
        sParams.swell = Mathf.Clamp(
            spectrumSettings.swell, 
            0.01F, 
            1
        );
        sParams.alpha = JonswapAlpha(
            gravity,
            spectrumSettings.fetch,
            spectrumSettings.windSpeed
        );
        sParams.peakOmega = JonswapPeakFrequency(
            gravity,
            spectrumSettings.fetch,
            spectrumSettings.windSpeed
        );
        sParams.gamma = spectrumSettings.peakEnhancement;
        sParams.shortWavesFade = spectrumSettings.shortWavesFade;

        SpectrumParameters[] paramsArray =
        {
            sParams
        };
        
        // Create a compute buffer to send to the shader
        _paramsBuffer = new ComputeBuffer(1, sizeof(float) * 8);
        _paramsBuffer.SetData(paramsArray);

        // Find our "Main" function
        _kernelID = computeShader.FindKernel("CalculateConjugatedSpectrum");
        
        // Feed shader noise texture
        computeShader.SetTexture(_kernelID, _noiseID, noiseTexture);
        
        // Set output textures
        computeShader.SetTexture(_kernelID, _resultID, renderTexture);
        computeShader.SetTexture(_kernelID, _waveDataTexID, wavesDataTexture);
        
        // Set spectrum params
        computeShader.SetBuffer(_kernelID, _spectrumsID, _paramsBuffer);
        
        // Set float params
        computeShader.SetFloat(_cutoffLowID, lowCutoff);
        computeShader.SetFloat(_cutoffHighID, highCutoff);
        computeShader.SetFloat(_gravityID, gravity);
        computeShader.SetFloat(_lengthScaleID, lengthScale);
        computeShader.SetFloat(_depthID, depth);

        // Dispatch ID
        int groups = Mathf.CeilToInt(resolution / 8.0F);
        computeShader.Dispatch(_kernelID, groups, groups, 1);
    }

    private void OnDestroy()
    {
        _paramsBuffer.Release();
        _paramsBuffer = null;
    }

    float JonswapAlpha(float g, float fetch, float windSpeed)
    {
        return 0.076F * Mathf.Pow(g * fetch / windSpeed / windSpeed, -0.22F);
    }

    float JonswapPeakFrequency(float g, float fetch, float windSpeed)
    {
        return 22.0F * Mathf.Pow(windSpeed * fetch / g / g, -0.33F);
    }
}
