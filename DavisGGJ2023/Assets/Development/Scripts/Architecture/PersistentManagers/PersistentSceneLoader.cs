using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentSceneLoader : DescriptionMonoBehavior
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private LoadGameLevelFuncChannelSO askLoadGameLevel;
    [SerializeField] private LoadSceneFuncChannelSO askLoadScene;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private TransitionOutFuncChannelSO askTransitionOut;
    [SerializeField] private TransitionInFuncChannelSO askTransitionIn;
    [SerializeField] private VoidEventChannelSO onGameLevelLoaded;

    private Dictionary<int, string> currentScenes = new();

    private void OnEnable()
    {
        askLoadGameLevel.OnCalled += LoadGameLevel;
        askLoadScene.OnCalled += LoadScene;
    }
    
    private void OnDisable()
    {
        askLoadGameLevel.OnCalled -= LoadGameLevel;
        askLoadScene.OnCalled -= LoadScene;
    }

    private bool LoadScene(
        string sceneName, 
        int sceneLayer, 
        TransitionEffect transitionOut, 
        TransitionEffect transitionIn) 
        => LoadScene(sceneName, sceneLayer, transitionOut, transitionIn, false);
    
    private bool LoadScene(
        string sceneName, 
        int sceneLayer, 
        TransitionEffect transitionOut, 
        TransitionEffect transitionIn, bool isGameLevel)
    {
        // Check if scene is already loaded
        if (currentScenes.ContainsKey(sceneLayer) && currentScenes[0] == sceneName)
        {
            return false;
        }

        StartCoroutine(CoroutLoadScene(sceneName, sceneLayer, transitionOut, transitionIn, isGameLevel));
        return true;
    }
    
    private bool LoadGameLevel(GameLevelSO level, TransitionEffect transitionOut, TransitionEffect transitionIn)
    {
        return LoadScene(level.name, 0, transitionOut, transitionIn, true);
    }

    private IEnumerator CoroutLoadScene(string newSceneName, int sceneLayer, TransitionEffect transitionOut, TransitionEffect transitionIn, bool isGameLevel)
    {
        // Start transition out

        yield return askTransitionOut.CallFunc(transitionOut, false);
        
        // Loading unloading scenes
        var unloadHandler = new AsyncOperation();
        bool existingScene = currentScenes.ContainsKey(sceneLayer);
        if (existingScene)
        {
            unloadHandler = SceneManager.UnloadSceneAsync(currentScenes[sceneLayer]);
        }
        
        var loadHandler = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

        loadHandler.completed += (operation) =>
        {
            currentScenes.TryAdd(sceneLayer, newSceneName);
            currentScenes[sceneLayer] = newSceneName;
        };
        
        if(existingScene)
            yield return unloadHandler;

        yield return loadHandler;

        if (isGameLevel)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
            onGameLevelLoaded.RaiseEvent();
        }
        
        // Start transition in
        yield return askTransitionIn.CallFunc(transitionIn, false);
    }
    
}
