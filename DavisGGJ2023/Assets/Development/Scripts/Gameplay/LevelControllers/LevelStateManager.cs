using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LevelStateManager : MonoBehaviour
{
    [ColorHeader("Listening", ColorHeaderColor.ListeningChannels)]
    [SerializeField] private VoidEventChannelSO askRestartLevel;
    [SerializeField] private VoidEventChannelSO onLevelLoaded;

    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private TransitionOutFuncChannelSO askGetTransitionOut;
    [SerializeField] private TransitionInFuncChannelSO askGetTransitionIn;
    [SerializeField] private InputModeEventChannelSO askChangeInputMode;
    [SerializeField] private VoidEventChannelSO askSpawnAllEntities;
    [SerializeField] private VoidEventChannelSO askClearAllEntities;
    [SerializeField] private VoidEventChannelSO askDiposeAllVFX;

    // Fields
    
    private Coroutine currentOperation;

    private void OnEnable()
    {
        onLevelLoaded.OnRaised += Setup;
        askRestartLevel.OnRaised += RestartLevel;
    }
    
    private void OnDisable()
    {
        onLevelLoaded.OnRaised -= Setup;
        askRestartLevel.OnRaised -= RestartLevel;
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
        askChangeInputMode.RaiseEvent(InputMode.Gameplay);
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
        
        askDiposeAllVFX.RaiseEvent();
        askClearAllEntities.RaiseEvent();
        LoadLevel();
        
        yield return askGetTransitionIn.CallFunc(TransitionEffect.FadeBlack, false);
        askChangeInputMode.RaiseEvent(InputMode.Gameplay);
        currentOperation = null;
    }

    private void LoadLevel()
    {
        askSpawnAllEntities.RaiseEvent();
    }
}
