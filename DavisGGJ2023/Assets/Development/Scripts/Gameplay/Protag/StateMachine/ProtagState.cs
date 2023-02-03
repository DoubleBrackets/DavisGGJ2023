using MushiSimpleFSM;
using UnityEngine;

public abstract class ProtagState : GenericState<ProtagBlackboard>
{
    protected ProtagState(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    protected ProtagTransitions transitions => GetTransitionTable<ProtagTransitions>();

    protected ProtagInputProvider inputProvider => blackboard.InputProvider;
    protected PlayerInputState inputState => blackboard.InputState;
    protected BasicMovement protagMover => blackboard.ProtagMover;
    protected BasicMovementProfileSO basicMovementProfile => blackboard.basicMovementProfile;
    protected HeightBody2D heightBody => blackboard.heightBody;
    protected ProtagAnimator animator => blackboard.animator;
}
