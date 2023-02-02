using MushiSimpleFSM;
using UnityEngine;

public class ProtagWalking : ProtagState
{
    public ProtagWalking(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        return transitions.WalkingToIdle(ref c);
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        
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
