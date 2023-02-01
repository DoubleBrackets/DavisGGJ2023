#region

using MushiCore.Editor;
using UnityEditor;
using UnityEngine;

#endregion

[CreateAssetMenu(menuName = "MushiStuff/MushiEditorTools/Hierarchy Overlays/Icons/HierarchyIconMonoScriptTarget")]
public class HierarchyIconMonoScriptTargetSO : HierarchyIconSO
{
    [ColorHeader("Target")]
    public MonoScript targetClass;
}