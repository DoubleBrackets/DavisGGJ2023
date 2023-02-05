using MushiSimpleFSM;
using UnityEngine;

public class ProtagStaggered : ProtagState
{
    public ProtagStaggered(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        if (transitions.AirborneToFalling(ref c))
        {
            return true;
        }

        if (stateDuration >= blackboard.recentlyHit.KnockbackStaggerDuration)
        {
            c = GetState<ProtagIdle>();
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        transitions.SubOnHazardDoDie();
        // Knockback
        var attack = blackboard.recentlyHitInfo;
        var attackProfile = blackboard.recentlyHit;
        
        heightBody.horizontalVel = 
            attack.attackAngle * Vector2.right * (attackProfile.KnockbackVelocity * attack.knockbackRatio);

        heightBody.verticalVelocity = attackProfile.VerticalKnockbackVelocity * attack.knockbackRatio;

    }

    public override void ExitState()
    {
        // Semi - Hard stop when leaving staggered
        if (heightBody.isGrounded)
        {
            heightBody.horizontalVel *= 0.6f;
        }
        transitions.UnsubOnHazardDoDie();
    }

    public override void UpdateState()
    {
        animator.SetFacing(heightBody.horizontalVel);
        animator.PlayAnimation("Idle");
    }

    public override void FixedUpdateState()
    {
        
    }
}
