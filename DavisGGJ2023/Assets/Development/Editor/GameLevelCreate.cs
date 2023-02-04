using System.IO;
using MushiCore.Editor;
using UnityEditor;
using UnityEngine;

public class GameLevelCreate 
{
    static readonly string resourcePath = "Assets/GeneralAssets/Scenes/TemplateLevel.unity";
    
    [MenuItem("Assets/Create/Gameplay/New Gameplay Level")]
    public static void CreateGameplayLevel()
    {
        // Create the template scene
        string templatePath = $"{resourcePath}";
        AssetTemplateUtility.CreateNamedAssetFromTemplate(
            templatePath,
            "NewGameLevel.unity",
            onAssetCreated: OnCreateScene,
            creationIcon:"d_SceneAsset Icon"
        );

        void OnCreateScene(string sceneFilePath)
        {
            Debug.Log($"Creating a new Gameplay Level Scene from template at {sceneFilePath} using the template at {templatePath}");
            string folderPath = Path.GetDirectoryName(sceneFilePath);
            string levelName = Path.GetFileName(sceneFilePath).Split('.')[0];

            string sceneAssetGuid = AssetDatabase.GUIDFromAssetPath(sceneFilePath).ToString();

            void PreProcessSO(GameLevelSO levelScene)
            {
                levelScene.sceneName = levelName;
                levelScene.scenePath = $"{folderPath}/{levelName}.asset";
            }
            
            AssetCreateUtility.CreateScriptableObjectDirect<GameLevelSO>(
                $"{folderPath}/{levelName}.asset",
                PreProcessSO
            );
        }
    }
}
