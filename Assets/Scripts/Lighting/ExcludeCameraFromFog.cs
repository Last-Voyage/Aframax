/*****************************************************************************
// File Name :         ExcludeCameraFromFog.cs
// Author :            Miles Rogers
// Creation Date :     4/29/2025
//
// Brief Description : Evil trick to exclude a single camera from rendering fog
*****************************************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Excludes a camera from rendering fog
/// </summary>
[RequireComponent(typeof(Camera))]
public class ExcludeCameraFromFog : MonoBehaviour
{
    /// <summary>
    /// Initial fog setting in the scene the camera exists in
    /// </summary>
    private bool _initialFogSetting = true;

    /// <summary>
    /// Reference to the attached camera
    /// </summary>
    private Camera _thisCamera;

    /// <summary>
    /// Cache initial fog setting on start
    /// </summary>
    private void Start()
    {
        _thisCamera = GetComponent<Camera>();
        _initialFogSetting = RenderSettings.fog; // Grab initial fog setting value
        
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
        RenderPipelineManager.endCameraRendering += EndCameraRendering;
    }

    /// <summary>
    /// Callback function from URP
    /// </summary>
    private void BeginCameraRendering(ScriptableRenderContext context, Camera renderCamera)
    {
        // Disable fog
        if (renderCamera == _thisCamera)
        {
            RenderSettings.fog = false;
        }
    }

    /// <summary>
    /// Callback function from URP
    /// </summary>
    private void EndCameraRendering(ScriptableRenderContext context, Camera renderCamera)
    {
        // Return fog to default value
        if (renderCamera == _thisCamera)
        {
            RenderSettings.fog = _initialFogSetting;
        }
    }
}
