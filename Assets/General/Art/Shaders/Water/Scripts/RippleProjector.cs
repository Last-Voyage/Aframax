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
using UnityEngine.Serialization;

/// <summary>
/// Responsible for setting up the camera render textures, the compute
/// shader, and provides coordinates to the main water shader for the
/// ripple projection.
/// </summary>
public class RippleProjector : MonoBehaviour
{
    [SerializeField] private MeshRenderer _waterMesh;
    
    [SerializeField] private ComputeShader _computeShader;
    
    // The direct output from the camera
    [SerializeField] private RenderTexture _cameraTexture;
    
    // The blending output before it is blurred by the compute shader
    [SerializeField] private RenderTexture _preBlurTexture;
    
    // The final blur output from the compute shader
    [SerializeField] private RenderTexture _outputTexture;
    
    // The resolution of the render texture
    [SerializeField] private int _resolution = 128;
    
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

    private int _textureSizeID = Shader.PropertyToID("_TextureSize");
    private int _positionID = Shader.PropertyToID("_BoxCenter");
    private int _scaleID = Shader.PropertyToID("_BoxSize");

    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        _waterMaterial = _waterMesh.sharedMaterial;
        _lastPosition = transform.position;
        _lastScale = _camera.orthographicSize;
        
        // Send data to water shader
        
        _waterMaterial.SetInt(_textureSizeID, _resolution);
        UpdateShaderData();
        
        // Find our "Main" function
        _kernelID = _computeShader.FindKernel("WaterRipple");
        _blurKernelID = _computeShader.FindKernel("Blur");
        
        // Setup render textures
        _computeShader.SetTexture(_kernelID, _cameraTextureID, _cameraTexture);
        _computeShader.SetTexture(_kernelID, _preBlurTextureID, _preBlurTexture);
        _computeShader.SetTexture(_kernelID, _outputTextureID, _outputTexture);

        _computeShader.SetTexture(_blurKernelID, _preBlurTextureID, _preBlurTexture);
        _computeShader.SetTexture(_blurKernelID, _outputTextureID, _outputTexture);
        
        _computeShader.SetInt(_imageSizeID, _resolution);
        
        // Dispatch
        int groups = Mathf.CeilToInt(_resolution / 8.0F);
        _computeShader.Dispatch(_kernelID, groups, groups, 1);
        _computeShader.Dispatch(_blurKernelID, groups, groups, 1);
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
            int groups = Mathf.CeilToInt(_resolution / 8.0F);
            _computeShader.Dispatch(_kernelID, groups, groups, 1);
            _computeShader.Dispatch(_blurKernelID, groups, groups, 1);

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
