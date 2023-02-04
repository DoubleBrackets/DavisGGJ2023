using System;
using UnityEngine;

public class PersistentGameManager : DescriptionMonoBehavior
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private LoadGameLevelFuncChannelSO askLoadGameLevel;
    [SerializeField] private LoadSceneFuncChannelSO askLoadScene;
    
    [ColorHeader("Dependencies")]
    [SerializeField] private StartupDataBoardSO startupBoard;
    [SerializeField] private GameLevelSO defaultEntryLevel;

    void OnEnable()
    {
        LoadPersistentScenes();
        LoadEntryScene();
    }

    private void OnDisable()
    {
        startupBoard.EntryGameLevel = null;
    }

    private void LoadEntryScene()
    {
        if (startupBoard.EntryGameLevel == null)
        {
            askLoadGameLevel.CallFunc(
                defaultEntryLevel, 
                TransitionEffect.None, 
                TransitionEffect.FadeBlack);
        }
        else
        {
            askLoadGameLevel.CallFunc(
                startupBoard.EntryGameLevel, 
                TransitionEffect.None, 
                TransitionEffect.None);
        }
    }

    private void LoadPersistentScenes()
    {
        
    }
}
