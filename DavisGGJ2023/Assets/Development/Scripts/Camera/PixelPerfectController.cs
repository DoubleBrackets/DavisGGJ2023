using System;
using Cinemachine;
using UnityEngine;

public class PixelPerfectController : MonoBehaviour
{
    [ColorHeader("Dependencies")]
    [SerializeField] private CinemachineVirtualCamera[] cameras;
    [SerializeField] private Material[] pixelPerfectMaterial;

    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private Vector2Int pixelResolution;
    [SerializeField] private float pixelsPerUnit;
    
    private static readonly int pixelResolutionProperty = Shader.PropertyToID("_PixelResolution");
    private static readonly int ppuProperty = Shader.PropertyToID("_PixelsPerUnit");

    private void OnValidate()
    {
        RecalculateCameraSize();
    }

    private void RecalculateCameraSize()
    {
        Vector2 pixelRes = new Vector2(pixelResolution.x, pixelResolution.y);

        foreach (var pixelPerfectMat in pixelPerfectMaterial)
        {
            pixelPerfectMat.SetVector(pixelResolutionProperty, pixelRes);
            pixelPerfectMat.SetFloat(ppuProperty, pixelsPerUnit);
        }

        Vector2 cameraDimensions = pixelRes / pixelsPerUnit;
        
        foreach (var cam in cameras)
        {
            cam.m_Lens.OrthographicSize = cameraDimensions.y / 2f;
        }
    }
}
