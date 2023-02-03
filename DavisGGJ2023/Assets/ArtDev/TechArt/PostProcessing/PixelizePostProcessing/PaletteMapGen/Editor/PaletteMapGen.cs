using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "Test/PaletteMapGen")]
public class PaletteMapGen : ScriptableObject
{

    [ColorHeader("File Paths")]
    [SerializeField] private Texture2D paletteSource;
    [SerializeField] private string generatedMapPath;
    [SerializeField] private string generatedMapName;

    [ColorHeader("Palette Config")]
    [SerializeField] private bool limitPaletteSize;
    [SerializeField] private int maxGeneratedPaletteSize;

    [SerializeField, Range(0f, 5f)] private float redWeight;
    [SerializeField, Range(0f, 5f)] private float greenWeight;
    [SerializeField, Range(0f, 5f)] private float blueWeight;
    
    [ColorHeader("Output Config")]
    [SerializeField] private int size = 64;
    [SerializeField] private int anisioLevel = 0;

    [SerializeField] private TextureFormat format = TextureFormat.RGBA32;
    [SerializeField] private TextureWrapMode wrapMode = TextureWrapMode.Clamp;
    [SerializeField] private FilterMode filterMode = FilterMode.Point;
    
    [ColorHeader("Compute")]
    [SerializeField] private ComputeShader paletteMapComputeShader;

    private ComputeBuffer result;
    private ComputeBuffer palette;

    public void CreateTexture3D()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        int paletteWidth = paletteSource.width;
        int paletteHeight = paletteSource.height;
        
        // Get palette colors
        var colorDict = new Dictionary<Color, int>();
        for (int x = 0; x < paletteWidth; x++)
        {
            for (int y = 0; y < paletteHeight; y++)
            {
                Color c = paletteSource.GetPixel(x, y, 0);
                colorDict.TryAdd(c, 0);
                colorDict[c]++;
            }
        }

        int paletteLength = colorDict.Count;

        var ordered = colorDict.OrderByDescending(a => a.Value);

        if (limitPaletteSize)
            paletteLength = Mathf.Min(paletteLength, maxGeneratedPaletteSize);

        paletteLength = Mathf.Min(paletteLength, 16000);
        
        var paletteColorArr = new Color[paletteLength];

        int i = 0;
        foreach (var color in ordered)
        {
            if (i >= paletteLength) break;
            paletteColorArr[i] = color.Key;
            i++;
        }
        
        // For Direct checking RGB
        palette = new ComputeBuffer(paletteLength, sizeof(float) * 4);
        palette.SetData(paletteColorArr);

        Texture2D paletteTex = new Texture2D(paletteLength, 1, TextureFormat.RGBA32, 0, false);
        paletteTex.wrapMode = TextureWrapMode.Clamp;
        paletteTex.filterMode = FilterMode.Point;
        paletteTex.anisoLevel = 0;
        
        paletteTex.SetPixels(paletteColorArr);
        paletteTex.Apply();

        Debug.Log("Palette texture created: " + stopwatch.ElapsedMilliseconds / 1000f);
        stopwatch.Restart();
        
        result = new ComputeBuffer(size * size * size, sizeof(float) * 4);
        
        // Set parameters
        paletteMapComputeShader.SetTexture(0, "_PaletteTexture", paletteTex);
        paletteMapComputeShader.SetInt("_PaletteWidth", paletteLength);
        paletteMapComputeShader.SetInt("_PaletteHeight", 1);
        paletteMapComputeShader.SetInt("_PaletteCount", paletteLength);
        paletteMapComputeShader.SetInt("_Size", size);
        paletteMapComputeShader.SetFloat("_rWeight", redWeight);
        paletteMapComputeShader.SetFloat("_gWeight", greenWeight);
        paletteMapComputeShader.SetFloat("_bWeight", blueWeight);

        paletteMapComputeShader.SetBuffer(0, "_Result", result);
        paletteMapComputeShader.SetBuffer(0, "_Palette", palette);
        

        // Calculate kernel group count
        int blockSize = 16;
        int xy = Mathf.CeilToInt((float)size / blockSize);
        int groupX = xy;
        int groupY = xy;
        int groupZ = size;
        
        // Run compute shader
        paletteMapComputeShader.Dispatch(0, groupX, groupY, groupZ);

        // Copy the color values to the texture
        var colors = new Color[size * size * size];
        result.GetData(colors);
        
        Debug.Log("Palette map created: " + stopwatch.ElapsedMilliseconds / 1000f);
        
        // Create the texture and apply the configuration
        var existingMap = AssetDatabase.LoadAssetAtPath<Texture3D>($"{generatedMapPath}/{generatedMapName}x{size}.asset");
        Texture3D texture = (existingMap == null) 
            ? new Texture3D(size, size, size, format, false) 
            : existingMap;
        
        texture.filterMode = filterMode;
        texture.anisoLevel = anisioLevel;
        texture.wrapMode = wrapMode;

        
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();        

        // Save the texture to your Unity Project
        if(existingMap == null)
            AssetDatabase.CreateAsset(texture, $"{generatedMapPath}/{generatedMapName}x{size}.asset");
        
        result.Release();
        result = null;
    }
}

[CustomEditor(typeof(PaletteMapGen))]
public class PaletteMapGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Palette 3D Texture Map"))
        {
            ((PaletteMapGen)target).CreateTexture3D();
        }
    }
}
