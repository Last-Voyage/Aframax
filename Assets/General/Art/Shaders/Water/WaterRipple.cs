using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRipple : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    
    [SerializeField] private RenderTexture cameraTexture;
    [SerializeField] private RenderTexture preBlurTexture;
    [SerializeField] private RenderTexture outputTexture;
    
    [SerializeField] private int resolution = 128;
    
    private int _cameraTextureID = Shader.PropertyToID("Camera");
    private int _preBlurTextureID = Shader.PropertyToID("PreBlur");
    private int _outputTextureID = Shader.PropertyToID("Result");
    
    private int _kernelID = 0;
    private int _blurKernelID = 0;

    private void Start()
    {
        // Find our "Main" function
        _kernelID = computeShader.FindKernel("WaterRipple");
        _blurKernelID = computeShader.FindKernel("Blur");
        
        // Setup render textures
        computeShader.SetTexture(_kernelID, _cameraTextureID, cameraTexture);
        computeShader.SetTexture(_kernelID, _preBlurTextureID, preBlurTexture);
        computeShader.SetTexture(_kernelID, _outputTextureID, outputTexture);

        computeShader.SetTexture(_blurKernelID, _preBlurTextureID, preBlurTexture);
        computeShader.SetTexture(_blurKernelID, _outputTextureID, outputTexture);
        
        // Dispatch
        int groups = Mathf.CeilToInt(resolution / 8.0F);
        computeShader.Dispatch(_kernelID, groups, groups, 1);
        computeShader.Dispatch(_blurKernelID, groups, groups, 1);
    }

    private float _updateDuration = 0.02F;
    private float _lastUpdateTime = 0.0F;

    private void Update()
    {
        if (_lastUpdateTime > _updateDuration)
        {
            int groups = Mathf.CeilToInt(resolution / 8.0F);
            computeShader.Dispatch(_kernelID, groups, groups, 1);
            computeShader.Dispatch(_blurKernelID, groups, groups, 1);

            _lastUpdateTime = 0.0F;
        }
        else
        {
            _lastUpdateTime += Time.deltaTime;
        }
    }
}
