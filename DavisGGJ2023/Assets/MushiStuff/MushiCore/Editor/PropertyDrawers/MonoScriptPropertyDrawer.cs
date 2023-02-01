#region

using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    [CustomPropertyDrawer(typeof(MonoScript))]
    public class MonoScriptPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MonoScript target = (MonoScript)property.objectReferenceValue;
            property.objectReferenceValue = EditorGUI.ObjectField(position, property.displayName, target, typeof(MonoScript), false);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}