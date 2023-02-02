void CalculatePixelUV_float(
    float2 uv, float2 pixelResolution, float2 finalUVOffset,
    out float2 pixelizedUV, out float2 pixelCoords)
{
    float2 pixelCoordsRaw = uv * pixelResolution;
    pixelCoords = floor(pixelCoordsRaw);
    pixelizedUV = pixelCoords / pixelResolution;
    pixelizedUV += finalUVOffset;
    //pixelizedUV = uv + finalUVOffset;
}