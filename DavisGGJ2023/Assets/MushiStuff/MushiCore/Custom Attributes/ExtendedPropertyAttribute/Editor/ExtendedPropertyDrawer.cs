#region

using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    [CustomPropertyDrawer(typeof(ExtendedPropertyAttribute))]
    public class ExtendedPropertyDrawer : PropertyDrawer
    {
        private bool foldedOut = false;
        private bool hasEntered = false;

        private GUIStyle backgroundStyle;
        private GUIStyle backgroundStyle2;

        private const float rightIndentWidth = 6f;

        // Padding values
        private const float paddingObjectToBackground = 3;
        private const float paddingBackgroundPanelToNestedInspector = 3;
        private const float paddingNestedInspectorToPanelBottom = 2;
        private const float paddingBackgroundToEnd = 2;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ExtendedPropertyAttribute)attribute;

            if (!hasEntered)
            {
                foldedOut = attr.shownByDefault;
                hasEntered = true;
                backgroundStyle = GUI.skin.FindStyle("RL Background");
                backgroundStyle2 = GUI.skin.FindStyle("RL Empty Header");
            }

            // Default object field and foldout
            Rect objectField = position;
            objectField.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(objectField, property);
            foldedOut = EditorGUI.Foldout(objectField, foldedOut, "");

            // Offset by object field height
            position.y += EditorGUI.GetPropertyHeight(property);


            if (foldedOut && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                DrawExtendedProperty(position, property, attr);
            }
        }

        private void DrawExtendedProperty(Rect position, SerializedProperty property, ExtendedPropertyAttribute attr)
        {
            var target = property.objectReferenceValue;
            if (target != null)
            {
                var serializedTarget = new SerializedObject(target);
                // Padding - Between Object field and Background Panel
                position.y += EditorGUIUtility.standardVerticalSpacing * paddingObjectToBackground;

                // Calculate background panel size
                Rect backgroundRect = position;
                float totalBackgroundPadding = paddingBackgroundPanelToNestedInspector + paddingNestedInspectorToPanelBottom;
                backgroundRect.height = CalculateNestedInspectorHeight(serializedTarget) + EditorGUIUtility.standardVerticalSpacing * totalBackgroundPadding;
                backgroundRect = EditorGUI.IndentedRect(backgroundRect);

                // Draw background
                GUI.Box(backgroundRect, "", backgroundStyle);
                GUI.Box(backgroundRect, "", backgroundStyle2);

                // Padding - Between top of Background Panel and start of Nested GUI
                position.y += EditorGUIUtility.standardVerticalSpacing * paddingBackgroundPanelToNestedInspector;

                // Draw nested inspector in indented block
                using (new EditorGUI.IndentLevelScope())
                {
                    // Indent right side as well
                    position.width -= rightIndentWidth;
                    // Draw nested inspector in Disabled scope for read only
                    using (new EditorGUI.DisabledScope(attr.readOnly))
                    {
                        NestedInspectorGUI(position, serializedTarget);
                    }
                }

                serializedTarget.Dispose();
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property);

            if (foldedOut && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var target = property.objectReferenceValue;

                float totalFoldoutPadding =
                    paddingObjectToBackground +
                    paddingBackgroundPanelToNestedInspector +
                    paddingNestedInspectorToPanelBottom +
                    paddingBackgroundToEnd;

                if (foldedOut && target != null)
                {
                    var serializedTarget = new SerializedObject(target);
                    float bodyHeight = CalculateNestedInspectorHeight(serializedTarget);
                    float paddingHeight = EditorGUIUtility.standardVerticalSpacing * totalFoldoutPadding;
                    height += bodyHeight + paddingHeight;
                    serializedTarget.Dispose();
                }
            }

            return height;
        }

        private float CalculateNestedInspectorHeight(SerializedObject serializedTarget)
        {
            var propIter = GetFirstPropertyField(serializedTarget);

            float height = 0;
            do
            {
                height += EditorGUI.GetPropertyHeight(propIter, new GUIContent(propIter.displayName), true);
                height += EditorGUIUtility.standardVerticalSpacing;
            } while (propIter.NextVisible(false));

            return height;
        }

        private void NestedInspectorGUI(Rect position, SerializedObject serializedTarget)
        {
            // Get reference to first valid property
            var propIter = GetFirstPropertyField(serializedTarget);
            serializedTarget.Update();

            // Loop through visible properties and display
            do
            {
                float height = EditorGUI.GetPropertyHeight(propIter, new GUIContent(propIter.displayName), true);
                var propRect = position;
                propRect.height = height;
                EditorGUI.PropertyField(propRect, propIter, true);
                position.y += height + EditorGUIUtility.standardVerticalSpacing;
            } while (propIter.NextVisible(false));

            serializedTarget.ApplyModifiedProperties();
        }

        private SerializedProperty GetFirstPropertyField(SerializedObject so)
        {
            var propIter = so.GetIterator();
            // Skip script field
            propIter.NextVisible(true);
            propIter.NextVisible(false);
            return propIter;
        }
    }
}