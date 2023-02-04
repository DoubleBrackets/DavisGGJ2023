#if UNITY_EDITOR
using System;
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
        string levelSOName = AssetDatabase.FindAssets($"{thisSceneName} t:GameLevelSO")[0];
        startDataBoard.EntryGameLevel = AssetDatabase.LoadAssetAtPath<GameLevelSO>(AssetDatabase.GUIDToAssetPath(levelSOName));
        
        // Start loading persistent managers
        var operation = SceneManager.LoadSceneAsync(persistentManagersSceneName, LoadSceneMode.Single);
    }


}
#endif