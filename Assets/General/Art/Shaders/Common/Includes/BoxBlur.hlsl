void BoxBlur_float(
    UnityTexture2D Texture,
    int TexSize,
    float2 UV, 
    float BlurAmount, 
    out float Output)
{    
    float sum = 0.0F;

    for (int u = -BlurAmount; u < BlurAmount; u++)
    {
        for (int v = -BlurAmount; v < BlurAmount; v++)
        {
            float uCoord = UV.x + (1.0F / TexSize) * u;
            float vCoord = UV.y + (1.0F / TexSize) * v;
            
            sum += tex2Dlod(Texture, float4(uCoord, vCoord, 0.0F, 0.0F));
        }
    }

    float mean = sum / (BlurAmount * BlurAmount);

    Output = mean;
}