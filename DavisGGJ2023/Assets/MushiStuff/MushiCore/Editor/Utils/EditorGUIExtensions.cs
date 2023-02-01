#region

using System.Reflection;
using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    /// <summary>
    /// Some helper methods for editor tools
    /// </summary>
    [InitializeOnLoad]
    public static class EditorGUIExtensions
    {
        /// <summary>
        /// Current total EditorGUI indent space
        /// </summary>
        public static float TotalIndentWidth => EditorGUI.indentLevel * IndentWidth;

        /// <summary>
        /// Width of each indent level
        /// </summary>
        public const float IndentWidth = 15f;

        public const float HierarchySingleLineHeight = 16f;


        /// <summary>
        /// Normal Editor window background color
        /// </summary>
        public static Color NormalWindowBackgroundColor;


        static EditorGUIExtensions()
        {
            var method = typeof(EditorGUIUtility).GetMethod(
                "GetDefaultBackgroundColor",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (method != null)
            {
                NormalWindowBackgroundColor = (Color)method.Invoke(null, null);
            }
            else
            {
                // Hard coded color
                NormalWindowBackgroundColor = EditorGUIUtility.isProSkin
                    ? new Color32(56, 56, 56, 255)
                    : new Color32(194, 194, 194, 255);
            }
        }
    }
}