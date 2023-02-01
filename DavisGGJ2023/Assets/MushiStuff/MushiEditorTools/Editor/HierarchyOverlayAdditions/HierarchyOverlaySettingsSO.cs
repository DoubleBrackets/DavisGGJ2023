#region

using System;
using System.Collections.Generic;
using MushiCore.Editor;
using UnityEditor;
using UnityEngine;

#endregion

public class HierarchyOverlaySettingsSO : ScriptableObject
{
    public const string defaultCreationPath = "Assets/EditorPreferenceSettings/Editor/HierarchyOverlaySettings.asset";
        
    private static HierarchyOverlaySettingsSO instance;

    public static HierarchyOverlaySettingsSO SettingsInstance
    {
        get
        {
            if (instance == null || instance.name == "")
            {
                instance = CreateDefaultSettings();
            }
            return instance;
        }
    }

    public void OnValidate()
    {
        instance = this;
    }

    public static HierarchyOverlaySettingsSO CreateDefaultSettings()
    {
        var settings = AssetCreateUtility.CreateScriptableObjectDirect<HierarchyOverlaySettingsSO>(defaultCreationPath, DefaultSettings);

        void DefaultSettings(HierarchyOverlaySettingsSO newSettings)
        {
            newSettings.drawSeparators = true;
            newSettings.separatorBackgroundColor = new Color(0.18f, 0.18f, 0.18f);
            newSettings.separatorTextColor = new Color(0.98f, 0.51f, 0.51f);

            newSettings.separatorIcon = new MultiSourceEditorIcon();
            newSettings.separatorIcon.iconType = MultiSourceEditorIcon.IconType.EditorGUIUtility;
            newSettings.separatorIcon.editorIconStringIdentifier = "d_tab_next@2x";

            newSettings.drawAlternatingLines = true;
            newSettings.alternatingLineOverlay = new Color(0.1f, 0.07f, 0.07f, 40/255f);

            newSettings.drawNestingLines = true;
            newSettings.nestingLineColor = new Color(0.47f, 0.46f, 0.46f);

            newSettings.drawIcons = true;
            newSettings.iconShrinkAmount = 1f;
            newSettings.iconOverlays = new();
        }
        
        return settings;
    }

    
    [Serializable]
    public struct IconOverlayGroup
    {
        public bool isActive;
        public HierarchyIconOverlayGroupSO group;
    }

    [ColorHeader("Separator")]
    public bool drawSeparators;

    public Color separatorBackgroundColor;
    public Color separatorTextColor;
    public MultiSourceEditorIcon separatorIcon;

    [ColorHeader("Alternating lines")]
    public bool drawAlternatingLines;

    public Color alternatingLineOverlay;

    [ColorHeader("Nesting Lines")]
    public bool drawNestingLines;

    public Color nestingLineColor;

    [ColorHeader("Icons")]
    public bool drawIcons;

    public float iconShrinkAmount;
    public List<IconOverlayGroup> iconOverlays;
}

[CustomEditor(typeof(HierarchyOverlaySettingsSO))]
public class HierarchyOverlayConfigDrawer : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Refresh Hierarchy Overlay"))
        {
            HierarchyItemOverlayDrawer.RefreshHierarchy();
        }
        base.OnInspectorGUI();
    }
}

public static class HierarchOverlaySettingsPreferences
{
    [SettingsProvider]
    public static SettingsProvider CreateHierarchyOverlaySettingsProvider()
    {
        var provider = new SettingsProvider("Preferences/MushiStuff/HierarchyOverlay", SettingsScope.User);
        provider.guiHandler = (searchContext) =>
        {
            var newDrawer = ScriptableObject.CreateInstance<HierarchyOverlayConfigDrawer>();
            EditorGUILayout.ObjectField("Current Settings", newDrawer, typeof(ScriptableObject), false);
        };
        return provider;
    }
}