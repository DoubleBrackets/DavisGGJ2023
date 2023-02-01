#region

using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    [CustomPropertyDrawer(typeof(RangeSliderAttribute))]
    public class RangeSliderDrawer : PropertyDrawer
    {
        private const float fieldWidth = 32f;
        private const float sliderMargins = 6f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangeSliderAttribute range = (RangeSliderAttribute)attribute;

            // Some values need to be adjusted for indentation
            float adjustedFieldWidth = fieldWidth + EditorGUIExtensions.TotalIndentWidth;
            float adjustedLabelWidth = EditorGUIUtility.labelWidth - EditorGUIExtensions.TotalIndentWidth;

            // Set up drawing rectangles
            Rect minValueField = position;
            minValueField.width = adjustedFieldWidth;
            minValueField.x += adjustedLabelWidth;

            Rect maxValueField = position;
            maxValueField.width = adjustedFieldWidth;
            maxValueField.x += position.width - adjustedFieldWidth;

            Rect sliderRect = position;
            sliderRect.x += adjustedLabelWidth + fieldWidth + sliderMargins;
            sliderRect.width -= adjustedLabelWidth + 2 * (fieldWidth + sliderMargins);

            Rect labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth;

            // Draw label
            EditorGUI.LabelField(labelRect, property.displayName);

            void DisplaySliders(ref float minVal, ref float maxVal)
            {
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, range.min, range.max);

                // Draw number fields for lower and upper values
                minVal = EditorGUI.FloatField(minValueField, minVal);
                maxVal = EditorGUI.FloatField(maxValueField, maxVal);

                // Clamp values
                minVal = Mathf.Clamp(minVal, range.min, maxVal);
                maxVal = Mathf.Clamp(maxVal, minVal, range.max);
            }

            // Different value processing for different types
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 floatSource = property.vector2Value;
                float min = floatSource.x;
                float max = floatSource.y;
                DisplaySliders(ref min, ref max);
                property.vector2Value = new Vector2(min, max);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                Vector2Int intSource = property.vector2IntValue;
                float min = intSource.x;
                float max = intSource.y;
                DisplaySliders(ref min, ref max);
                property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(min), Mathf.RoundToInt(max));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}