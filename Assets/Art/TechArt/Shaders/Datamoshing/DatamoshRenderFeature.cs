using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DatamoshRenderFeature : ScriptableRendererFeature
{
    private DatamoshRenderPass _datamoshPass;
    
    [SerializeField] private Material _datamoshMaterial;

    private RTHandle _previousFrameTexHandle;
    private RTHandle _previousColors;
    
    public override void AddRenderPasses(
        ScriptableRenderer renderer, 
        ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            renderer.EnqueuePass(_datamoshPass);
        }
    }
    
    public override void SetupRenderPasses(
        ScriptableRenderer renderer, 
        in RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            _datamoshPass.ConfigureInput(ScriptableRenderPassInput.Color 
                | ScriptableRenderPassInput.Motion);
            _datamoshPass.SetTarget(renderer.cameraColorTargetHandle);
        }
    }
    
    public override void Create()
    {
        RTHandles.Initialize(Screen.width, Screen.height);
        RTHandles.SetReferenceSize(Screen.width, Screen.height);

        _previousFrameTexHandle = RTHandles.Alloc(Screen.width, Screen.height);
        _datamoshPass = new DatamoshRenderPass(
            _datamoshMaterial,
            _previousFrameTexHandle
        );
    }
}
