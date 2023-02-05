using System;
using MushiSimpleFSM;
using UnityEngine;

public class ProtagMainController : DescriptionMonoBehavior
{
    [SerializeField] private ProtagBlackboard blackboard;

    private StateMachine<ProtagBlackboard> playerStateMachine;
    
    // Debug
    [ReadOnly, SerializeField] private string currentState;

    private void OnEnable()
    {
        playerStateMachine = new ProtagStateMachine(blackboard);
        playerStateMachine.InitializeEntryState<ProtagIdle>();
        blackboard.InputProvider.Events.OnPausePressed += DebugReset;
        blackboard.askSetDamping.RaiseEvent(0f);
        blackboard.askStartFollowingTarget.RaiseEvent(blackboard.playerBodyTransform);

        blackboard.gameState.ProtagBlackboard = blackboard;
    }

    private void Start()
    {
        blackboard.askSetDamping.RaiseEvent(1f);
    }

    private void OnDisable()
    {
        playerStateMachine.DisableStateMachine();
        blackboard.InputProvider.Events.OnPausePressed -= DebugReset;
        blackboard.gameState.ProtagBlackboard = null;
    }

    private void DebugReset()
    {
        Debug.Log(playerStateMachine.Blackboard);
        blackboard.askRestartLevel.RaiseEvent();
    }

    private void Update()
    {
        blackboard.UpdateInput();
        playerStateMachine.Update();
        currentState = playerStateMachine.CurrentState.ToString();
    }

    private void FixedUpdate()
    {
        playerStateMachine.FixedUpdate();
    }
}
