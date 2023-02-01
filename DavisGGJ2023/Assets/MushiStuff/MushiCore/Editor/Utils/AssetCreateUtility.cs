#region

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

#endregion

namespace MushiCore.Editor
{
    /// <summary>
    /// Utility class for directly creating assets. For creating from templates, look at AssetTemplateUtility>
    /// </summary>
    public static class AssetCreateUtility
    {
        /// <summary>
        /// Create a scriptable object directly
        /// </summary>
        /// <param name="filePath">Path where asset will be created. Should include file name</param>
        /// <param name="preProcessAction">Any actions to preprocess the SO before creating asset file</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateScriptableObjectDirect<T>(string filePath, Action<T> preProcessAction) where T : ScriptableObject
        {
            T newInstance = ScriptableObject.CreateInstance<T>();
            preProcessAction(newInstance);
            CreateFoldersRecursive(filePath, true);
            AssetDatabase.CreateAsset(newInstance, filePath);
            AssetDatabase.SaveAssets();
            return newInstance;
        }

        public static void CreateFoldersRecursive(string folderPath, bool trimLast = false)
        {
            var directories = folderPath.Split('/');
            string start = directories[0];
            for (int i = 1; i < directories.Length - (trimLast ? 1 : 0); i++)
            {
                if(!AssetDatabase.IsValidFolder(start + "/" + directories[i]))
                    AssetDatabase.CreateFolder(start, directories[i]);
                start += "/" + directories[i];
            }
        }

        /// <summary>
        /// Write content to a text file and imports it as an asset
        /// </summary>
        /// <param name="assetPath">Asset path(Relative to asset folder)</param>
        public static void WriteTextAsset(string assetPath, string content)
        {
            string fullGeneratePath = Path.GetFullPath(assetPath);
            var encoding = new UTF8Encoding(true);
            CreateFoldersRecursive(assetPath, true);
            File.WriteAllText(fullGeneratePath, content, encoding);
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.SaveAssets();
        }
    }
}
