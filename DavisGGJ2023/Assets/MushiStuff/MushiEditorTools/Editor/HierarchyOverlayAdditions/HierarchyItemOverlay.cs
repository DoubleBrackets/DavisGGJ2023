#region

using UnityEngine;

#endregion

/// <summary>
/// Base class for drawing a type of item overlay in the hierarchy
/// </summary>
public abstract class HierarchyItemOverlay
{
    public abstract void DrawHierarchyItem(GameObject gameObject, Rect selectionRect, HierarchyOverlaySettingsSO settings);

    public abstract bool ShouldDrawForItem(GameObject gameObject);
}