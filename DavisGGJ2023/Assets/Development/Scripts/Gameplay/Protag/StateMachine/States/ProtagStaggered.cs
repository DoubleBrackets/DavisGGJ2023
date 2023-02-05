using MushiSimpleFSM;
using UnityEngine;

public class ProtagStaggered : ProtagState
{
    public ProtagStaggered(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        if (stateDuration >= blackboard.recentAttacked.KnockbackStaggerDuration)
        {
            c = GetState<ProtagIdle>();
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        transitions.SubOnHazardDoDie();
        blackboard.protagCombatEntity.onAttackReceived += ChainHit;
        Knockback();
    }

    private bool ChainHit(AttackProfileSO attackProfileSo, AttackInfo attackInfo)
    {
        blackboard.recentAttackedInfo = attackInfo;
        blackboard.recentAttacked = attackProfileSo;

        stateEntryTime = Time.time;
        Knockback();
        return true;
    }
    
    private void Knockback()
    {
        // Knockback
        var attack = blackboard.recentAttackedInfo;
        var attackProfile = blackboard.recentAttacked;
        
        heightBody.horizontalVel = 
            attack.attackAngle * Vector2.right * (attackProfile.KnockbackVelocity * attack.knockbackRatio);

        heightBody.verticalVelocity = attackProfile.VerticalKnockbackVelocity * attack.knockbackRatio;

        blackboard.askFreezeFrame.RaiseEvent(0.2f);
    }

    public override void ExitState()
    {
        // Semi - Hard stop when leaving staggered
        if (heightBody.isGrounded)
        {
            heightBody.horizontalVel *= 0.6f;
        }
        transitions.UnsubOnHazardDoDie();blackboard.protagCombatEntity.onAttackReceived -= ChainHit;
    }

    public override void UpdateState()
    {
        animator.SetFacing(heightBody.horizontalVel);
        animator.PlayAnimation(heightBody.isGrounded ? "Idle" : "Falling");
    }

    public override void FixedUpdateState()
    {
        
    }
}
