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
    }

    private void OnDisable()
    {
        playerStateMachine.DisableStateMachine();
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
