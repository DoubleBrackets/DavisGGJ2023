using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LevelStateManager : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private VoidEventChannelSO askRestartLevel;
    [SerializeField] private VoidEventChannelSO onLevelLoaded;
    [SerializeField] private LevelEntranceEventChannelSO askTravelToNewLevel;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private TransitionOutFuncChannelSO askGetTransitionOut;
    [SerializeField] private TransitionInFuncChannelSO askGetTransitionIn;
    [SerializeField] private InputModeEventChannelSO askChangeInputMode;
    [SerializeField] private BoolEventChannelSO askSpawnAllEntities;
    [SerializeField] private LevelEntranceEventChannelSO askSpawnPlayer;
    [SerializeField] private VoidEventChannelSO askClearAllEntities;
    [SerializeField] private VoidEventChannelSO askDiposeAllVFX;
    [SerializeField] private LoadGameLevelFuncChannelSO askLoadGameLevel;

    [ColorHeader("Dependencies")]
    [SerializeField] private GameStateSO gameState;
    // Fields
    
    private Coroutine currentOperation;

    private void OnEnable()
    {
        gameState.CurrentEnemyCount = 0;
        onLevelLoaded.OnRaised += Setup;
        askRestartLevel.OnRaised += RestartLevel;
        askTravelToNewLevel.OnRaised += TravelToNewLevel;
    }
    
    private void OnDisable()
    {
        onLevelLoaded.OnRaised -= Setup;
        askRestartLevel.OnRaised -= RestartLevel;
        askTravelToNewLevel.OnRaised -= TravelToNewLevel;
    }

    private void TravelToNewLevel(LevelEntranceSO nextLevel)
    {
        var previousLevel = gameState.CurrentlyLoadedLevel;
        askChangeInputMode.RaiseEvent(InputMode.Disabled);
        bool successful = askLoadGameLevel.CallFunc(
            nextLevel.LevelToEnter,
            TransitionEffect.FadeBlack,
            TransitionEffect.FadeBlack,
            Cleanup);

        if (successful)
        {
            gameState.LevelCleared(previousLevel);
            gameState.TargetEntrance = nextLevel;
        }
            
    }

    private void Setup()
    {
        BeginLevel();
    }

    private void BeginLevel()
    {
        if (currentOperation != null) return;
        askChangeInputMode.RaiseEvent(InputMode.Disabled);
        currentOperation = StartCoroutine(CoroutBeginLevel());
    }

    private IEnumerator CoroutBeginLevel()
    {
        LoadLevel();
        yield return new WaitForSeconds(0.5f);
        currentOperation = null;
    }

    private void RestartLevel()
    {
        if (currentOperation != null) return;

        currentOperation = StartCoroutine(CoroutRestartLevel());
    }

    private IEnumerator CoroutRestartLevel()
    {
        askChangeInputMode.RaiseEvent(InputMode.Disabled);
        yield return askGetTransitionOut.CallFunc(TransitionEffect.FadeBlack, false);

        Cleanup();
        LoadLevel();
        
        yield return askGetTransitionIn.CallFunc(TransitionEffect.FadeBlack, false);
        currentOperation = null;
    }

    private void Cleanup()
    {
        askDiposeAllVFX.RaiseEvent();
        askClearAllEntities.RaiseEvent();
    }

    private void LoadLevel()
    {
        askSpawnPlayer.RaiseEvent(gameState.TargetEntrance);
        
        askSpawnAllEntities.RaiseEvent(gameState.IsCurrentLevelCleared());
    }
}
