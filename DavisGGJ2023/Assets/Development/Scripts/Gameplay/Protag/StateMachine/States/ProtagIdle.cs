using MushiSimpleFSM;
using UnityEngine;

public class ProtagIdle : ProtagState
{
    public ProtagIdle(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        return 
            transitions.AirborneToFalling(ref c) || 
            transitions.IdleToWalking(ref c);
    }

    public override void EnterState()
    {
        animator.PlayAnimation("Idle");
        transitions.SubBasicActions();
    }

    public override void ExitState()
    {
        transitions.UnsubBasicActions();
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        protagMover.SimpleHorizontalMovement(
            inputState.movementVector,
            basicMovementProfile.MaxWalkSpeed,
            basicMovementProfile.WalkAcceleration,
            basicMovementProfile.FrictionAcceleration,
            Time.fixedDeltaTime
            );
    }
}
