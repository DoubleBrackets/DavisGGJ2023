#region

using MushiCore.Editor;
using UnityEngine;

#endregion

[CreateAssetMenu(menuName = "MushiStuff/MushiEditorTools/Hierarchy Overlays/Icons/HierarchyIconStringTarget")]
public class HierarchyIconStringTargetSO : HierarchyIconSO
{
    [ColorHeader("Target")]
    public string targetClassString;
}