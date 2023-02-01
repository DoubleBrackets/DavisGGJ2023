#region

using System;
using System.Collections.Generic;
using MushiCore.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

/// <summary>
/// Controlling class for drawing hierarchy item overlays
/// </summary>
[InitializeOnLoad]
public class HierarchyItemOverlayDrawer
{
    // Internal class for cache data
    private class ItemOverlayData
    {
        public List<HierarchyItemOverlay> overlays = new();
        public List<HierarchyIconSO> iconOverlays = new();

        public bool isDirty = true;

        // Nesting data
        public bool isLastChild;
        public bool isFirstChild;
        public bool hasChildren;
    }

    // Cache for overlay data about each Hierarchy gameObject
    private static Dictionary<GameObject, ItemOverlayData> ItemDataCache = new();

    private static bool Initialized;

    private static HierarchyOverlaySettingsSO Settings => HierarchyOverlaySettingsSO.SettingsInstance;

    private static HierarchyItemOverlay SeparatorOverlay;

    static HierarchyItemOverlayDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        EditorApplication.hierarchyChanged += MarkCacheAsDirty;
        EditorSceneManager.sceneOpened += SceneOpened;
    }

    // Some hard coded sizing values
    private static float nestingLineTopPadding = 2f;

    /// <summary>
    /// Mark each cache item as dirty
    /// </summary>
    private static void MarkCacheAsDirty()
    {
        foreach (var pair in ItemDataCache)
        {
            pair.Value.isDirty = true;
        }
    }

    /// <summary>
    /// Clear cache when a new scene (and thus new hierarchy gameObjects) are opened
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private static void SceneOpened(Scene scene, OpenSceneMode mode)
    {
        ItemDataCache.Clear();
    }

    /// <summary>
    /// Updates reference to active config and marks cache as dirty. Should be called when changing configs or manual refresh.
    /// Repaints hierarchy for immediate feedback
    /// </summary>
    public static void RefreshHierarchy()
    {
        MarkCacheAsDirty();
        EditorApplication.RepaintHierarchyWindow();
    }

    /// <summary>
    /// Initialize, should only run once per script reload on the first draw frame
    /// </summary>
    private static void Initialize()
    {
        SeparatorOverlay = new HierarchySeparatorOverlay();
        RefreshHierarchy();
        Initialized = true;
    }

    /// <summary>
    /// Main draw function
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="pos"></param>
    private static void HierarchyWindowItemOnGUI(int instanceID, Rect pos)
    {
        // Initialize during the first draw call, since individual overlays may need access to IMGUI methods
        if (!Initialized)
        {
            Initialize();
        }

        var gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        if (gameObject != null && Settings != null)
        {
            // Create cache entry if non exists
            if (!ItemDataCache.ContainsKey(gameObject))
            {
                ItemDataCache.Add(gameObject, new ItemOverlayData());
            }

            // Rebuild overlay cache for gameObject if dirty
            if (ItemDataCache[gameObject].isDirty)
            {
                BuildObjectOverlayCache(gameObject);
            }

            DrawOverlays(gameObject, pos);
        }
    }

    /// <summary>
    /// Draw overlays for a hierarchy item
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="pos"></param>
    private static void DrawOverlays(GameObject gameObject, Rect pos)
    {
        if (Settings.drawAlternatingLines)
            DrawAlternatingLineOverlay(pos);

        if (Settings.drawNestingLines)
            DrawNestingLine(gameObject, pos);

        // Draw overlays present in object cache
        foreach (var overlay in ItemDataCache[gameObject].overlays)
        {
            overlay.DrawHierarchyItem(gameObject, pos, Settings);
        }

        if (Settings.drawIcons)
            DrawIconOverlays(ItemDataCache[gameObject].iconOverlays, pos);
    }

    /// <summary>
    /// Build overlay data for a single cache item
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="selectionRect"></param>
    private static void BuildObjectOverlayCache(GameObject gameObject)
    {
        var cacheData = ItemDataCache[gameObject];
        cacheData.isDirty = false;

        // Clear cached data
        cacheData.overlays.Clear();
        cacheData.iconOverlays.Clear();

        // Nesting data
        if (gameObject.transform.parent != null)
        {
            int siblingIndex = gameObject.transform.GetSiblingIndex();
            int totalSiblings = gameObject.transform.parent.childCount;

            cacheData.isLastChild = (siblingIndex == totalSiblings - 1);
            cacheData.isFirstChild = siblingIndex == 0;
            cacheData.hasChildren = gameObject.transform.childCount > 0;
        }

        // Add separator overlay if valid
        if (SeparatorOverlay.ShouldDrawForItem(gameObject))
        {
            cacheData.overlays.Add(SeparatorOverlay);
        }

        // Icon overlays
        var components = gameObject.GetComponents<Component>();
        Type classReferenceType;
        Type componentType;

        // Iterate over each component
        foreach (var component in components)
        {
            if (component == null) continue;

            componentType = component.GetType();

            // Check if the component is a valid target of some icon overlay
            bool matchFound = false;

            foreach (var iconOverlayGroupConfig in Settings.iconOverlays)
            {
                if (!iconOverlayGroupConfig.isActive || iconOverlayGroupConfig.group == null) continue;

                var iconOverlayGroup = iconOverlayGroupConfig.group;

                // Match the string targets

                string className = componentType.Name.ToLower();

                if (iconOverlayGroup.stringTargetDict.ContainsKey(className))
                {
                    cacheData.iconOverlays.Add(iconOverlayGroup.stringTargetDict[className]);
                    matchFound = true;
                    break;
                }

                // Match the monoscript targets

                foreach (var monoScriptIcon in iconOverlayGroup.monoScriptTargetIcons)
                {
                    if (monoScriptIcon == null) continue;

                    classReferenceType = monoScriptIcon.targetClass.GetClass();

                    if (!classReferenceType.IsClass) continue;

                    // Check if the component is assignable to or a subclass of the icon target class
                    if (classReferenceType.IsAssignableFrom(componentType) || componentType.IsSubclassOf(classReferenceType))
                    {
                        cacheData.iconOverlays.Add(monoScriptIcon);
                        matchFound = true;
                        break;
                    }
                }

                // Only one icon drawn per component
                if (matchFound)
                {
                    break;
                }
            }
        }
    }

    private static void DrawAlternatingLineOverlay(Rect pos)
    {
        int count = (int)(pos.y / EditorGUIExtensions.HierarchySingleLineHeight);
        if (count % 2 == 0)
        {
            pos.x = 0;
            pos.width = EditorGUIUtility.currentViewWidth;
            EditorGUI.DrawRect(pos, Settings.alternatingLineOverlay);
        }
    }

    private static void DrawNestingLine(GameObject gameObject, Rect pos)
    {
        var parentTransform = gameObject.transform.parent;

        if (parentTransform == null)
            return;

        var cacheData = ItemDataCache[gameObject];
        pos.x -= EditorGUIExtensions.IndentWidth * 1.5f - 1;

        if (cacheData.isLastChild)
        {
            // Shortened vertical line
            Rect verticalLine = pos;
            verticalLine.width = 1f;
            verticalLine.height = EditorGUIExtensions.HierarchySingleLineHeight / 2f;

            EditorGUI.DrawRect(verticalLine, Settings.nestingLineColor);

            // Horizontal line
            Rect horizontalLine = pos;
            horizontalLine.height = 1f;
            horizontalLine.y += EditorGUIExtensions.HierarchySingleLineHeight / 2f;

            // Shortened horizontal line to make space for dropdown triangle
            horizontalLine.width = cacheData.hasChildren
                ? EditorGUIExtensions.IndentWidth / 2f
                : EditorGUIExtensions.IndentWidth - 2f;

            EditorGUI.DrawRect(horizontalLine, Settings.nestingLineColor);
        }
        else
        {
            // Non-last children just draw a simple vertical line
            Rect verticalLine = pos;
            verticalLine.width = 1f;

            // Extra spacing from top if first child
            if (cacheData.isFirstChild)
            {
                verticalLine.height -= nestingLineTopPadding;
                verticalLine.y += nestingLineTopPadding;
            }

            EditorGUI.DrawRect(verticalLine, Settings.nestingLineColor);
        }

        // Recursively draw the lines for parents
        Transform current = gameObject.transform.parent;

        if (current != null)
        {
            Rect verticalLine = pos;
            verticalLine.width = 1f;
            Transform currentParent = current.parent;
            while (currentParent != null)
            {
                verticalLine.x -= EditorGUIExtensions.IndentWidth - 1;

                if (ItemDataCache.ContainsKey(current.gameObject))
                {
                    var data = ItemDataCache[current.gameObject];
                    if (!data.isLastChild)
                    {
                        EditorGUI.DrawRect(verticalLine, Settings.nestingLineColor);
                    }
                }

                current = currentParent;
                currentParent = currentParent.parent;
            }
        }
    }

    private static void DrawIconOverlays(IList<HierarchyIconSO> iconOverlays, Rect pos)
    {
        pos.width = EditorGUIExtensions.HierarchySingleLineHeight - Settings.iconShrinkAmount;
        pos.height -= Settings.iconShrinkAmount;
        pos.y += Settings.iconShrinkAmount / 2f;

        // Draw starting from right side of window
        pos.x = EditorGUIUtility.currentViewWidth - pos.width - EditorGUIExtensions.IndentWidth;

        // Draw in reverse, so reads left-right in order of components in the gameObject
        for (int i = iconOverlays.Count - 1; i >= 0; i--)
        {
            var iconOverlay = iconOverlays[i];
            GUI.DrawTexture(pos, iconOverlay.icon);
            pos.x -= pos.width;
        }
    }

    [MenuItem("GameObject/SeparatorGameObject")]
    private static void CreateSeparatorObject()
    {
        EditorSceneManager.MarkAllScenesDirty();
        var created = new GameObject();
        created.name = "/Separator";
        created.isStatic = true;
        created.SetActive(false);
    }
}