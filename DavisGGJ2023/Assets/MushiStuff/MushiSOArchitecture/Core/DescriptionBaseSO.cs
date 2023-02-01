#region

using UnityEngine;

#endregion

public class DescriptionBaseSO : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField, TextArea] private string description;
#endif
}