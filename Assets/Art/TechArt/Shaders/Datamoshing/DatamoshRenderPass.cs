using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ProfilingScope = UnityEngine.Rendering.ProfilingScope;

public class DatamoshRenderPass : ScriptableRenderPass
{
    private ProfilingSampler _profileSampler = new("DatamoshEffect");

    private Material _mat;
    private bool _isFirstFrame = true;
    
    private RTHandle _dmTexHandle;
    private RTHandle _cameraColorHandle;
    private RTHandle _prevFrameTexHandle;

    private RenderTextureDescriptor _cameraDescriptor;

    public DatamoshRenderPass(Material mat, RTHandle prevFrameTexture)
    {
        _mat = mat;
        _prevFrameTexHandle = prevFrameTexture;
        
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public void SetTarget(RTHandle colorHandle)
    {
        _cameraColorHandle = colorHandle;
    }

    public override void OnCameraSetup(
        CommandBuffer cmd, 
        ref RenderingData renderingData)
    {
        _cameraColorHandle = 
            renderingData.cameraData.renderer.cameraColorTargetHandle;
        
        ConfigureTarget(new[]
        {
            _cameraColorHandle,
            _dmTexHandle
        });
    }

    public override void Configure(
        CommandBuffer cmd, 
        RenderTextureDescriptor cameraTextureDescriptor)
    {
        RenderTextureDescriptor desc = new(
            cameraTextureDescriptor.width,
            cameraTextureDescriptor.height
        );

        _dmTexHandle = RTHandles.Alloc(desc);
        _cameraDescriptor = cameraTextureDescriptor;
        
        Debug.Log("Configured DatamoshEffect");
    }

    public override void Execute(
        ScriptableRenderContext context, 
        ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        RTHandle camTexHandle = 
            renderingData.cameraData.renderer.cameraColorTargetHandle;

        using (new ProfilingScope(cmd, _profileSampler))
        {
            if (_isFirstFrame)
            {
                SetTarget(_prevFrameTexHandle);
                Blitter.BlitCameraTexture(
                    cmd, 
                    camTexHandle, 
                    _prevFrameTexHandle, 
                    _mat, 
                    1
                );
			
                _isFirstFrame = false;
            }
            else
            {
                if (_prevFrameTexHandle.IsUnityNull())
                {
                    return;
                }
                
                // Blit from camera to temp
                Blitter.BlitCameraTexture(
                    cmd, 
                    _prevFrameTexHandle, 
                    _dmTexHandle, 
                    _mat, 
                    1
                ); 
                
                // Blit from temp to camera
                Blitter.BlitCameraTexture(
                    cmd, 
                    _dmTexHandle, 
                    camTexHandle, 
                    _mat, 
                    0
                );
                
                // Blit to previous camera texture
                Blitter.BlitCameraTexture(
                    cmd, 
                    camTexHandle, 
                    _prevFrameTexHandle, 
                    _mat, 
                    1
                );
            }
        }
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
        _dmTexHandle.Release();
    }
}
