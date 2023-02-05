using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RootGameUtilWindow : EditorWindow
{
    [MenuItem("Tools/Root Game Utils")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<RootGameUtilWindow>("Root Game Utils");
    }

    private Vector3 castPos;
    private LayerMask terrainLayers;
    private LayerMask entityLayers;

    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
        terrainLayers = LayerMask.GetMask("TerrainWall", "TerrainGround");
        entityLayers = LayerMask.GetMask("Enemy", "Player", "Destroyable", "Roots");
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.Vector3Field("MousePos", castPos);
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        castPos = ray.origin;
        castPos.z = float.MinValue;

        Color cursorColor = Color.white;

        bool anyHits = false;
        
        // Terrain cast
        var hits = Physics2D.OverlapCircleAll(ray.origin, 0.1f, terrainLayers);
        foreach (var coll in hits)
        {
            float height = coll.bounds.center.z;
            var stairs = coll.GetComponent<VerticalHeightRamp>();
            if (stairs)
            {
                height = stairs.EvaluateHeight(ray.origin);
            }
            if (height > castPos.z)
            {
                castPos.z = height;
                cursorColor = Color.white;
            }

            anyHits = true;
        }
        
        hits = Physics2D.OverlapCircleAll(ray.origin, 0.1f, entityLayers);
        foreach (var coll in hits)
        {
            float height = coll.bounds.center.z;
            if (height > castPos.z)
            {
                castPos.z = height;
                cursorColor = Color.red;
            }

            anyHits = true;
        }
        
        if(anyHits)
            Repaint();

        Handles.color = cursorColor;
        Handles.DrawWireDisc(ray.origin, -Vector3.forward, 0.2f);

        Handles.color = Color.green;
        Handles.DrawWireDisc(castPos - Vector3.up * castPos.z, -Vector3.forward, 0.2f);
        
        // Repaint on mouse movement
        if (Event.current.type == EventType.MouseMove)
        {
            SceneView.RepaintAll();
        }
    }
}
