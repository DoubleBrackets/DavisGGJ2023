using MushiSimpleFSM;
using UnityEngine;

public class ProtagFalling : ProtagState
{
    public ProtagFalling(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        return transitions.GroundedToWalking(ref c);
    }

    public override void EnterState()
    {
        heightBody.horizontalVel.x = 0f;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        if (Time.time - stateEntryTime > 0.1f)
        {
            animator.SetFacing(ProtagAnimator.Facing.Down);
            animator.PlayAnimation("Falling");
        }
    }

    public override void FixedUpdateState()
    {

    }
}
