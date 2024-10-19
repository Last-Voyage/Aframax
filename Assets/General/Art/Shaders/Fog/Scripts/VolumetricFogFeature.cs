using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class VolumetricFogFeature : ScriptableRendererFeature
{
    public VolumetricFogSettings settings = new();
    private VolumetricFogPass pass;
    
    public override void Create()
    {
        pass = new VolumetricFogPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera != Camera.main)
        {
            return;
        }
        
        renderer.EnqueuePass(pass);
    }
    
    [System.Serializable]
    public sealed class VolumetricFogSettings
    {
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        public Material VolumetricFogMaterial;
        public bool InstantiateMaterial;
    }
}

public sealed class VolumetricFogPass : ScriptableRenderPass
{
    private static readonly List<FogVolume> FogVolumes = new List<FogVolume>();
    private static bool ShouldRender;

    private Material FogMaterialInstance;
    private MaterialPropertyBlock FogMaterialProperties;
    private BufferedRenderTargetReference BufferedFogRenderTarget;

    public VolumetricFogPass(VolumetricFogFeature.VolumetricFogSettings settings)
    {
        renderPassEvent = settings.Event;
        FogMaterialInstance = Object.Instantiate(settings.VolumetricFogMaterial);
        FogMaterialProperties = new MaterialPropertyBlock();
        BufferedFogRenderTarget = null;
    }
    
    public Color ColorNothing { get; set; }

    public override void OnCameraSetup(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        ShouldRender = FogVolumes.Exists(f => f.gameObject.activeInHierarchy);

        if (!ShouldRender)
        {
            return;
        }

        if (HasCameraResized(ref renderingData))
        {
            BufferedFogRenderTarget = BufferedFogRenderTarget ?? new BufferedRenderTargetReference("_BufferedVolumetricFogRenderTarget");
            BufferedFogRenderTarget.SetRenderTextureDescriptor(new RenderTextureDescriptor(
                renderingData.cameraData.cameraTargetDescriptor.width / 2,
                renderingData.cameraData.cameraTargetDescriptor.height / 2,
                RenderTextureFormat.ARGB32, 0, 1), FilterMode.Bilinear, TextureWrapMode.Clamp);
        }

        foreach (var fogVolume in FogVolumes)
        {
            if (!fogVolume.gameObject.activeInHierarchy)
            {
                continue;
            }
            
            fogVolume.Apply(FogMaterialProperties);
            
            RasterizeColorToTarget(
                commandBuffer, 
                BufferedFogRenderTarget.BackBuffer.Handle, 
                FogMaterialInstance,
                BlitGeometry.Quad, 
                0, 
                FogMaterialProperties
            );
        }
        
        BlitBlendOntoCamera(
            commandBuffer, 
            BufferedFogRenderTarget.BackBuffer.Handle, 
            ref renderingData
        );
        
        BufferedFogRenderTarget.Clear(commandBuffer, ColorNothing);
        FogMaterialProperties.SetMatrix(Shader.PropertyToID("_CameraNearPlaneCorners"), renderingData.cameraData.camera.projectionMatrix);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!ShouldRender)
        {
            return;
        }

        CommandBuffer commandBuffer = CommandBufferPool.Get("VolumetricFogPass");

        using (new ProfilingScope(commandBuffer, new ProfilingSampler("VolumetricFogPass")))
        {
            // Eventually dispatch draw commands here
        }
        
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
        
        CommandBufferPool.Release(commandBuffer);
    }

    public static void AddFogVolume(FogVolume volume)
    {
        RemoveFogVolume(volume);
        FogVolumes.Add(volume);
    }

    public static void RemoveFogVolume(FogVolume volume)
    {
        FogVolumes.RemoveAll(f => f == volume);
    }
    
    private int PreviousCameraWidth;
    private int PreviousCameraHeight;
    
    private bool HasCameraResized(ref RenderingData renderingData)
    {
        if ((renderingData.cameraData.camera.pixelWidth != PreviousCameraWidth) || 
            (renderingData.cameraData.camera.pixelHeight != PreviousCameraHeight))
        {
            PreviousCameraWidth = renderingData.cameraData.camera.pixelWidth;
            PreviousCameraHeight = renderingData.cameraData.camera.pixelHeight;

            return true;
        }

        return false;
    }
    
    public enum BlitGeometry
    {
        Triangle = 0,
        Quad = 1
    };
    
    protected static CustomMesh TriangleMesh;
    protected static CustomMesh QuadMesh;
    
    protected Material BlitBlendMaterial;
    protected Material BlitTransparencyDepthCopyMaterial;
    
    private void RasterizeColorToTarget(CommandBuffer commandBuffer, RTHandle colorTarget, Material materialOverride = null, BlitGeometry geometry = BlitGeometry.Triangle, int shaderPassIndex = 0, MaterialPropertyBlock properties = null)
    {
        var mesh = (geometry == BlitGeometry.Triangle ? TriangleMesh.Mesh : QuadMesh.Mesh);
        var material = materialOverride ?? BlitTransparencyDepthCopyMaterial;

        commandBuffer.SetRenderTarget(colorTarget);
        commandBuffer.DrawMesh(mesh, Matrix4x4.identity, material, 0, shaderPassIndex, properties);
    }
    
    protected void BlitBlendOntoCamera(CommandBuffer commandBuffer, RTHandle sourceHandle, ref RenderingData renderingData)
    {
        BlitBlendMaterial.SetTexture(
            Shader.PropertyToID("_MainTex"), 
            sourceHandle, 
            RenderTextureSubElement.Color
        );
        
        Blit(
            commandBuffer, 
            sourceHandle, 
            renderingData.cameraData.renderer.cameraColorTargetHandle, 
            BlitBlendMaterial
        );
    }
}
