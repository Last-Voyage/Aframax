using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Helper class for Unity's <see cref="RTHandle"/>.
/// </summary>
public sealed class RenderTargetReference
{
    public string Name { get; private set; }
    public int Id { get; private set; }
    public RTHandle Handle { get; private set; }

    public RenderTextureFormat Format { get; private set; }
    public FilterMode FilterMode { get; private set; }
    public TextureWrapMode WrapMode { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public RenderTargetReference(string name)
    {
        Name = name;
        Id = Shader.PropertyToID(name);
        Handle = RTHandles.Alloc(name);
    }

    public RenderTargetReference(string name, int width, int height, RenderTextureFormat colorFormat)
    {
        Name = name;
        Id = Shader.PropertyToID(name);
        SetRenderTextureDescriptor(width, height, colorFormat);
    }
    
    public RenderTargetReference(string name, RenderTextureDescriptor descriptor)
    {
        Name = name;
        Id = Shader.PropertyToID(name);
        SetRenderTextureDescriptor(descriptor);
    }
    
    public bool SetRenderTextureDescriptor(
        RenderTextureDescriptor descriptor, 
        FilterMode filterMode = FilterMode.Point, 
        TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if ((Width == descriptor.width) && (Height == descriptor.height) && (Format == descriptor.colorFormat) && (FilterMode == filterMode) && (WrapMode == wrapMode))
        {
            return false;
        }

        Release();

        Handle = RTHandles.Alloc(descriptor, filterMode, wrapMode, name: Name);
        Width = descriptor.width;
        Height = descriptor.height;
        Format = descriptor.colorFormat;
        FilterMode = filterMode;
        WrapMode = wrapMode;

        return true;
    }
    
    public bool SetRenderTextureDescriptor(
        int width, 
        int height, 
        RenderTextureFormat colorFormat, 
        int depthBits = 0, 
        FilterMode filterMode = FilterMode.Point, 
        TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if ((Width == width) && (Height == height) && (Format == colorFormat) && (FilterMode == filterMode) && (WrapMode == wrapMode))
        {
            return false;
        }

        Release();

        Handle = RTHandles.Alloc(new RenderTextureDescriptor(width, height, colorFormat, depthBits, 1), filterMode, wrapMode, name: Name);
        Width = width;
        Height = height;
        Format = colorFormat;
        FilterMode = filterMode;
        WrapMode = wrapMode;

        return true;
    }
    
    public void GetTemporaryRT(
        CommandBuffer commandBuffer, 
        RenderTextureDescriptor rtDescriptor, 
        Color clearColor)
    {
        commandBuffer.GetTemporaryRT(Id, rtDescriptor);
        Clear(commandBuffer, clearColor);
    }

    public void Clear(
        CommandBuffer commandBuffer, 
        Color clearColor, 
        float depth = 1.0f)
    {
        commandBuffer.SetRenderTarget(Handle);
        commandBuffer.ClearRenderTarget(true, true, clearColor, depth);
    }
    
    public void Clear(CommandBuffer commandBuffer)
    {
        commandBuffer.SetRenderTarget(Handle);
        commandBuffer.ClearRenderTarget(true, true, Color.black, 1.0f);
    }
    
    public void GetTemporaryRT(
        CommandBuffer commandBuffer, 
        RenderTextureDescriptor rtDescriptor, 
        FilterMode filterMode, 
        Color clearColor)
    {
        commandBuffer.GetTemporaryRT(Id, rtDescriptor, filterMode);
        commandBuffer.SetRenderTarget(Handle);
        commandBuffer.ClearRenderTarget(true, true, clearColor);
    }
    
    public void ReleaseTemporaryRT(CommandBuffer commandBuffer)
    {
        commandBuffer.ReleaseTemporaryRT(Id);
    }
    
    public void Release()
    {
        if (Handle != null)
        {
            RTHandles.Release(Handle);
        }
    }
}
