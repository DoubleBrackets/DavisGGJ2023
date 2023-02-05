using System;
using UnityEngine;

public class PersistentGameManager : DescriptionMonoBehavior
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private LoadGameLevelFuncChannelSO askLoadGameLevel;
    [SerializeField] private LoadSceneFuncChannelSO askLoadScene;
    
    [ColorHeader("Dependencies")]
    [SerializeField] private GameStateSO gameState;
    [SerializeField] private GameLevelSO defaultEntryLevel;

    void OnEnable()
    {
        LoadPersistentScenes();
        LoadEntryScene();
    }

    private void OnDisable()
    {
        gameState.ResetValues();
    }

    private void LoadEntryScene()
    {
        if (gameState.EntryGameLevel == null)
        {
            askLoadGameLevel.CallFunc(
                defaultEntryLevel, 
                TransitionEffect.None, 
                TransitionEffect.FadeBlack);
        }
        else
        {
            askLoadGameLevel.CallFunc(
                gameState.EntryGameLevel, 
                TransitionEffect.None, 
                TransitionEffect.None);
        }
    }

    private void LoadPersistentScenes()
    {
        
    }
}
