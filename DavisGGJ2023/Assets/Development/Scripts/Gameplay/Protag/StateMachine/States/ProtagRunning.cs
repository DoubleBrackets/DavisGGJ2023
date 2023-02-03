using MushiSimpleFSM;
using UnityEngine;

public class ProtagRunning : ProtagState
{
    public ProtagRunning(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        return transitions.AirborneToFalling(ref c)||
               transitions.WalkingToIdle(ref c);
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        animator.SetFacing(inputState.movementVector);
        animator.PlayAnimation(heightBody.horizontalVel.magnitude > 0.1f ? "Run" : "Idle");
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
