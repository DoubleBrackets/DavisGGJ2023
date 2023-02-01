#region

using System;
using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    /// <summary>
    /// Simple editor class representing gui content with a texture from multiple possible sources
    /// Intended for use in tool config SOs
    /// </summary>
    [Serializable]
    public class MultiSourceEditorIcon
    {
        public enum IconType
        {
            EditorGUIUtility,
            TextureAsset
        }

        // Public fields
        public IconType iconType;
        public string editorIconStringIdentifier;
        public Texture2D textureAsset;

        public GUIContent Content => GetContent();

        private GUIContent GetContent()
        {
            if (iconType == IconType.EditorGUIUtility)
            {
                return EditorGUIUtility.IconContent(editorIconStringIdentifier);
            }
            else
            {
                return new GUIContent(textureAsset);
            }
        }

        public static implicit operator GUIContent(MultiSourceEditorIcon icon)
        {
            return icon.GetContent();
        }

        public static implicit operator Texture(MultiSourceEditorIcon icon)
        {
            if (icon.iconType == IconType.EditorGUIUtility)
            {
                return icon.Content.image;
            }
            else
            {
                return icon.textureAsset;
            }
        }
    }

    [CustomPropertyDrawer(typeof(MultiSourceEditorIcon))]
    public class MultiSourceEditorIconDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = (MultiSourceEditorIcon)property.boxedValue;
            position.height = EditorGUIUtility.singleLineHeight;
            // Icon Type dropdown
            target.iconType = (MultiSourceEditorIcon.IconType)EditorGUI.EnumPopup(position, label, target.iconType);

            // Set up positions for icon data fields
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            // Built in icon string field
            if (target.iconType == MultiSourceEditorIcon.IconType.EditorGUIUtility)
            {
                target.editorIconStringIdentifier = EditorGUI.TextField(position, target.editorIconStringIdentifier);
            }
            // Custom texture asset field
            else if (target.iconType == MultiSourceEditorIcon.IconType.TextureAsset)
            {
                target.textureAsset = (Texture2D)EditorGUI.ObjectField(position, target.textureAsset, typeof(Texture), false);
            }

            property.boxedValue = target;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}