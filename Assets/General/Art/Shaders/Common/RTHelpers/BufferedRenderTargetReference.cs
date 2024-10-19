using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// A double-buffered <see cref="RenderTargetReference"/>.
/// </summary>
public class BufferedRenderTargetReference
{
    public RenderTargetReference FrontBuffer { get; private set; }
    public RenderTargetReference BackBuffer { get; private set; }
    
    private RenderTargetReference Target0;
    private RenderTargetReference Target1;
    
    public BufferedRenderTargetReference(string name)
    {
        Target0 = new RenderTargetReference(name + "0");
        Target1 = new RenderTargetReference(name + "1");

        FrontBuffer = Target0;
        BackBuffer = Target1;
    }
    
    public BufferedRenderTargetReference(
        string name, 
        int width, 
        int height, 
        RenderTextureFormat colorFormat)
    {
        Target0 = new RenderTargetReference(name + "0", width, height, colorFormat);
        Target1 = new RenderTargetReference(name + "1", width, height, colorFormat);

        FrontBuffer = Target0;
        BackBuffer = Target1;
    }

    public BufferedRenderTargetReference(
        string name, 
        RenderTextureDescriptor descriptor)
    {
        Target0 = new RenderTargetReference(name + "0", descriptor);
        Target1 = new RenderTargetReference(name + "1", descriptor);

        FrontBuffer = Target0;
        BackBuffer = Target1;
    }
    
    public void Swap()
    {
        RenderTargetReference temp = FrontBuffer;

        FrontBuffer = BackBuffer;
        BackBuffer = temp;
    }
    
    public bool SetRenderTextureDescriptor(
        RenderTextureDescriptor descriptor, 
        FilterMode filterMode = FilterMode.Bilinear, 
        TextureWrapMode wrapMode = TextureWrapMode.Clamp)
    {
        return (Target0.SetRenderTextureDescriptor(descriptor, filterMode, wrapMode) &&
                Target1.SetRenderTextureDescriptor(descriptor, filterMode, wrapMode));
    }
    
    public bool SetRenderTextureDescriptor(
        int width, 
        int height, 
        RenderTextureFormat colorFormat, 
        int depthBits = 0, 
        FilterMode filterMode = FilterMode.Bilinear, 
        TextureWrapMode wrapMode = TextureWrapMode.Clamp)
    {
        return (Target0.SetRenderTextureDescriptor(width, height, colorFormat, depthBits, filterMode, wrapMode) &&
                Target1.SetRenderTextureDescriptor(width, height, colorFormat, depthBits, filterMode, wrapMode));
    }
    
    public void Clear(
        CommandBuffer commandBuffer, 
        Color clearColor, 
        float depth = 1.0F)
    {
        BackBuffer.Clear(commandBuffer, clearColor, depth);
    }
    
    public void Clear(CommandBuffer commandBuffer)
    {
        BackBuffer.Clear(commandBuffer);
    }
}
