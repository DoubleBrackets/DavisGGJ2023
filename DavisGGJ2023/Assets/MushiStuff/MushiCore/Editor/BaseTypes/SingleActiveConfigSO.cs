#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace MushiCore.Editor
{
    /// <summary>
    /// Scriptable object in which only one instance can be "active" at a time
    /// Useful for editor tools which might want multiple config SOs as profiles
    /// These should always be in 'Editor' folders
    /// </summary>
    public abstract class SingleActiveConfigSO : ScriptableObject
    {
        // Subclasses should have a custom drawer for this that uses SetActiveConfig()
        [HideInInspector, SerializeField] private bool isActiveConfig;
        public bool IsActiveConfig => isActiveConfig;

        // Event dictionary for updating tools when active config changes
        private static Dictionary<Type, Action> ActiveConfigChangedEvents = new();

        private void OnValidate()
        {
            // Roundabout way of dealing with duplicating SOs
            // Prevents both duplicate and source objects from both being active
            SetActiveConfig(isActiveConfig);
        }

        /// <summary>
        /// Get all config SOs of type name
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static SingleActiveConfigSO[] GetAllConfigs(string typeName)
        {
            var guids = AssetDatabase.FindAssets($"t:{typeName}");
            var results = new SingleActiveConfigSO[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                results[i] = AssetDatabase.LoadAssetAtPath<SingleActiveConfigSO>(path);
            }

            return results;
        }

        /// <summary>
        /// Get all configs SOs of generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetAllConfigs<T>() where T : SingleActiveConfigSO
        {
            return GetAllConfigs(typeof(T).ToString()).Select(config => (T)config).ToArray();
        }

        /// <summary>
        /// Get the active config SO using type name
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static SingleActiveConfigSO GetActiveConfig(string typeName)
        {
            return GetAllConfigs(typeName).FirstOrDefault(config => config.IsActiveConfig);
        }

        /// <summary>
        /// Get the active config SO of generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetActiveConfig<T>() where T : SingleActiveConfigSO
        {
            return (T)GetAllConfigs(typeof(T).ToString()).FirstOrDefault(config => config.IsActiveConfig);
        }

        /// <summary>
        /// Set this config to be active, automatically setting the previously active config to be inactive
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveConfig(bool active)
        {
            var type = GetType();
            if (active)
            {
                var prevActiveConfig = GetActiveConfig(type.ToString());
                if (prevActiveConfig != null)
                {
                    prevActiveConfig.isActiveConfig = false;
                }

                isActiveConfig = true;
            }
            else
            {
                isActiveConfig = false;
            }

            if (ActiveConfigChangedEvents.ContainsKey(type))
            {
                ActiveConfigChangedEvents[type]?.Invoke();
            }
        }

        /// <summary>
        /// Add a listener for active config change events
        /// </summary>
        public static void AddConfigChangeListener<T>(Action listener) where T : SingleActiveConfigSO
        {
            var type = typeof(T);
            if (!ActiveConfigChangedEvents.ContainsKey(type))
            {
                ActiveConfigChangedEvents.TryAdd(typeof(T), listener);
            }
            else
            {
                ActiveConfigChangedEvents[type] += listener;
            }
        }

        /// <summary>
        /// Remove a listener for active config change events
        /// </summary>
        public static void RemoveConfigChangeListener<T>(Action listener) where T : SingleActiveConfigSO
        {
            var type = typeof(T);
            if (ActiveConfigChangedEvents.ContainsKey(type))
            {
                ActiveConfigChangedEvents[type] -= listener;
            }
        }

        /// <summary>
        /// Manually trigger the config changed event for some config type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void TriggerConfigChangeEvent<T>() where T : SingleActiveConfigSO
        {
            var type = typeof(T);
            if (ActiveConfigChangedEvents.ContainsKey(type))
            {
                ActiveConfigChangedEvents[type]?.Invoke();
            }
        }

        /// <summary>
        /// Utility function for drawing isActive field, special handling to make sure any changed configs are marked dirty
        /// </summary>
        /// <param name="targetConfig"></param>
        public static void DrawIsActiveField(SingleActiveConfigSO targetConfig)
        {
            bool prevValue = targetConfig.isActiveConfig;

            bool newVal = EditorGUILayout.Toggle("Is Active Config", prevValue);

            if (prevValue != newVal)
            {
                EditorUtility.SetDirty(targetConfig);

                // Mark previously active config as dirty, since it is force de-activated
                var prevActiveConfig = GetActiveConfig(targetConfig.GetType().ToString());

                if (prevActiveConfig != null && prevActiveConfig != targetConfig)
                {
                    EditorUtility.SetDirty(prevActiveConfig);
                }

                targetConfig.SetActiveConfig(newVal);
            }
        }
    }
}