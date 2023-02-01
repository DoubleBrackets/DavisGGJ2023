#region

using UnityEditor;
using UnityEngine;

#endregion

public class HierarchySeparatorOverlay : HierarchyItemOverlay
{
    private GUIStyle overlayStyle;
    private GUIContent iconContent;

    public HierarchySeparatorOverlay()
    {
        overlayStyle = new GUIStyle(EditorStyles.label);
        overlayStyle.alignment = TextAnchor.MiddleLeft;
    }

    public override void DrawHierarchyItem(GameObject gameObject, Rect selectionRect, HierarchyOverlaySettingsSO settings)
    {
        if (!settings.drawSeparators) return;

        selectionRect.x = 0;
        selectionRect.width = EditorGUIUtility.currentViewWidth;

        // Background
        EditorGUI.DrawRect(selectionRect, settings.separatorBackgroundColor);

        // Icon
        iconContent = settings.separatorIcon;

        // Text style
        overlayStyle.normal.textColor = settings.separatorTextColor;
        overlayStyle.hover.textColor = settings.separatorTextColor;

        // Setup text
        string displayName = gameObject.name.Trim('/');
        iconContent.text = displayName;

        EditorGUI.LabelField(selectionRect, iconContent, overlayStyle);
    }

    public override bool ShouldDrawForItem(GameObject gameObject)
    {
        string name = gameObject.name;
        return name[0] == '/';
    }
}