#region

using System.Collections.Generic;
using UnityEngine;

#endregion

/// <summary>
/// A group of HierarchyIconOverlays, used to organize configs
/// </summary>
[CreateAssetMenu(menuName = "MushiStuff/MushiEditorTools/Hierarchy Overlays/Icons/Overlay Group")]
public class HierarchyIconOverlayGroupSO : ScriptableObject
{
    public List<HierarchyIconStringTargetSO> stringTargetIcons;
    public List<HierarchyIconMonoScriptTargetSO> monoScriptTargetIcons;

    public Dictionary<string, HierarchyIconStringTargetSO> stringTargetDict;

    private void OnValidate()
    {
        stringTargetDict = new Dictionary<string, HierarchyIconStringTargetSO>();
        foreach (var stringTarget in stringTargetIcons)
        {
            if (!stringTarget) continue;
            stringTargetDict.TryAdd(stringTarget.targetClassString.ToLower(), stringTarget);
        }
    }
}