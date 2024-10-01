/**************************************************************************
// File Name :          RippleProjector.cs
// Author :             Miles Rogers
// Creation Date :      9/30/2024
//
// Brief Description :  Sets up the camera render textures, the compute
//                      shader, and provides coordinates to the main
//                      water shader. 
**************************************************************************/

using UnityEngine;

public class RippleProjector : MonoBehaviour
{
    [SerializeField] private MeshRenderer waterMesh;
    
    [SerializeField] private ComputeShader computeShader;
    
    // The direct output from the camera
    [SerializeField] private RenderTexture cameraTexture;
    
    // The blending output before it is blurred by the compute shader
    [SerializeField] private RenderTexture preBlurTexture;
    
    // The final blur output from the compute shader
    [SerializeField] private RenderTexture outputTexture;
    
    // The resolution of the render texture
    [SerializeField] private int resolution = 128;
    
    private int _cameraTextureID = Shader.PropertyToID("Camera");
    private int _preBlurTextureID = Shader.PropertyToID("PreBlur");
    private int _outputTextureID = Shader.PropertyToID("Result");
    private int _imageSizeID = Shader.PropertyToID("ImageSize");
    
    private int _kernelID = 0;
    private int _blurKernelID = 0;

    private Camera _camera;
    private Material _waterMaterial;
    
    private Vector3 _lastPosition;
    private float _lastScale;

    private int _positionID = Shader.PropertyToID("_BoxCenter");
    private int _scaleID = Shader.PropertyToID("_BoxSize");

    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        _waterMaterial = waterMesh.sharedMaterial;
        _lastPosition = transform.position;
        _lastScale = _camera.orthographicSize;
        
        // Set RT dimensions
        cameraTexture.width = resolution;
        cameraTexture.height = resolution;

        preBlurTexture.width = resolution;
        preBlurTexture.height = resolution;

        outputTexture.width = resolution;
        outputTexture.height = resolution;
        
        UpdateShaderData();
        
        // Find our "Main" function
        _kernelID = computeShader.FindKernel("WaterRipple");
        _blurKernelID = computeShader.FindKernel("Blur");
        
        // Setup render textures
        computeShader.SetTexture(_kernelID, _cameraTextureID, cameraTexture);
        computeShader.SetTexture(_kernelID, _preBlurTextureID, preBlurTexture);
        computeShader.SetTexture(_kernelID, _outputTextureID, outputTexture);

        computeShader.SetTexture(_blurKernelID, _preBlurTextureID, preBlurTexture);
        computeShader.SetTexture(_blurKernelID, _outputTextureID, outputTexture);
        
        computeShader.SetInt(_imageSizeID, resolution);
        
        // Dispatch
        int groups = Mathf.CeilToInt(resolution / 8.0F);
        computeShader.Dispatch(_kernelID, groups, groups, 1);
        computeShader.Dispatch(_blurKernelID, groups, groups, 1);
    }
    
    // Used for tracking how long it has been since the last update.
    private float _updateDuration = 0.02F;
    private float _lastUpdateTime = 0.0F;

    private void Update()
    {
        // Only update when position / frustum gets updated
        if (transform.position != _lastPosition)
        {
            _lastPosition = transform.position;
            UpdateShaderData();
        }

        if (Mathf.Abs(_camera.orthographicSize - _lastScale) > 0.01F)
        {
            _lastScale = _camera.orthographicSize;
            UpdateShaderData();
        }
        
        // Re-dispatch compute work
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

    /// <summary>
    /// Updates the water shader with the proper coordinates/camera
    /// frustum bounds.
    /// </summary>
    private void UpdateShaderData()
    {
        _waterMaterial.SetVector(
            _positionID, 
            new Vector4(
                _lastPosition.x, 
                _lastPosition.y, 
                _lastPosition.z, 
                0.0F
        ));
        
        _waterMaterial.SetVector(
            _scaleID, 
            new Vector4(
                _lastScale,
                _lastScale,
                _lastScale,
                0.0F
        ));
    }
}
