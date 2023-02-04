#region

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace MushiCore.Editor
{
    /// <summary>
    /// Contains the color header color settings. Only one should be active at a time 
    /// </summary>
    public class ColorHeaderSettingsSO : ScriptableObject
    {
        public const string defaultCreationPath = "Assets/EditorPreferenceSettings/Editor/ColorHeaderSettings.asset";
        
        private static ColorHeaderSettingsSO instance;

        public static ColorHeaderSettingsSO SettingsInstance
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
        
        public static ColorHeaderSettingsSO CreateDefaultSettings()
        {
            var settings = AssetCreateUtility.CreateScriptableObjectDirect<ColorHeaderSettingsSO>(defaultCreationPath, DefaultColors);

            void DefaultColors(ColorHeaderSettingsSO newSettings)
            {
                newSettings.headerColors = new Color[7];
                var headerColors = newSettings.headerColors;
                headerColors[0] = new Color(0.6f, 0.98f, 1f);
                headerColors[1] = new Color(1f, 0.72f, 0.55f);
                headerColors[2] = new Color(0.98f, 1f, 0.55f);
                headerColors[3] = new Color(0.9f, 0.43f, 0.45f);
                headerColors[4] = new Color(0.98f, 1f, 0.55f);
                headerColors[5] = new Color(0.49f, 0.71f, 1f);
                headerColors[6] = new Color(0.6f, 0.98f, 1f);
            }
            return settings;
        }

        
        public Color[] headerColors;

        private void OnValidate()
        {
            instance = this;
        }
        
        private void OnEnable()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public Color GetHeaderColor(ColorHeaderColor color)
        {
            return headerColors[(int)color];
        }
        
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(SettingsInstance);
        }

        public void InspectorGUI()
        {
            var colorNames = Enum.GetNames(typeof(ColorHeaderColor));
            int newColorCount = colorNames.Length;
            var targetConfig = this;

            Undo.RecordObject(targetConfig, "Change SCHeader config");

            // Draw color fields for each enum color field 
            int currentColorCount = targetConfig.headerColors.Length;

            // If mismatch due to enum updates, then update color field array
            if (newColorCount != currentColorCount)
            {
                UpdateColorFields(targetConfig, newColorCount, currentColorCount);
            }

            // Draw color fields for each  element
            for (int i = 0; i < newColorCount; i++)
            {
                Color curr = targetConfig.headerColors[i];
                targetConfig.headerColors[i] = EditorGUILayout.ColorField(colorNames[i], curr);
                if (curr != targetConfig.headerColors[i])
                    EditorUtility.SetDirty(targetConfig);
            }
        }
        
        private void UpdateColorFields(ColorHeaderSettingsSO targetSO, int newColorCount, int oldColorCount)
        {
            var newColorFields = new Color[newColorCount];

            // Populate with old values
            float populateLength = Mathf.Min(oldColorCount, newColorCount);
            for (int i = 0; i < populateLength; i++)
            {
                newColorFields[i] = targetSO.headerColors[i];
            }

            // Fill in rest with white
            for (int i = oldColorCount; i < newColorCount; i++)
            {
                newColorFields[i] = Color.white;
            }

            // Update field to new list
            targetSO.headerColors = newColorFields;
        }
    }

    [CustomEditor(typeof(ColorHeaderSettingsSO))]
    public class ColorHeaderSettingsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ((ColorHeaderSettingsSO)target).InspectorGUI();
        }
    }

    public static class ColorHeaderSettingsPreferences
    {
        [SettingsProvider]
        public static SettingsProvider CreateColorHeaderSettingsProvider()
        {
            var provider = new SettingsProvider("Preferences/MushiStuff/ColorHeader", SettingsScope.User);
            provider.guiHandler = (searchContext) => ColorHeaderSettingsSO.SettingsInstance.InspectorGUI();
            return provider;
        }
    }
}