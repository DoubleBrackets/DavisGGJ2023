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
        transitions.SubOnHitDoDie();
    }

    public override void ExitState()
    {
        transitions.UnsubOnHitDoDie();
        blackboard.askSetDamping.RaiseEvent(1f);
    }

    public override void UpdateState()
    {
        blackboard.askSetDamping.RaiseEvent(1 + Mathf.Pow(5*stateDuration, 2f));
        if (stateDuration > 0.1f)
        {
            animator.SetFacing(heightBody.horizontalVel);
            animator.PlayAnimation("Falling");
        }
    }

    public override void FixedUpdateState()
    {

    }
}
