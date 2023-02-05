
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ColdStartup : MonoBehaviour
{

    [ColorHeader("Dependencies")]
    [SerializeField] private GameStateSO startDataBoard;
    
    [ColorHeader("Config", ColorHeaderColor.Config)]
    [SerializeField] private string persistentManagersSceneName;

    private string thisSceneName;
#if UNITY_EDITOR
    private void OnEnable()
    {
        var persistentManagerScene = SceneManager.GetSceneByName(persistentManagersSceneName);
        if (!persistentManagerScene.IsValid())
        {
            PerformColdStartup();
        }
    }

    private void PerformColdStartup()
    {
        // Make sure to mark this level as to be loaded
        var scene = SceneManager.GetActiveScene();
        thisSceneName = scene.name;
        var levelSONames = AssetDatabase.FindAssets($"{thisSceneName} t:GameLevelSO");
        foreach (var guid in levelSONames)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            if (sceneName == thisSceneName)
            {
                startDataBoard.EntryGameLevel = AssetDatabase.LoadAssetAtPath<GameLevelSO>(path);
            }
        }

        // Start loading persistent managers
        var operation = SceneManager.LoadSceneAsync(persistentManagersSceneName, LoadSceneMode.Single);
    }

#endif
}
