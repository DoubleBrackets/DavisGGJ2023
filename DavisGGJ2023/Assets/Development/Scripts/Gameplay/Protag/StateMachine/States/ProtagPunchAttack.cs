using MushiSimpleFSM;
using UnityEngine;

public class ProtagPunchAttack : ProtagState
{
    public ProtagPunchAttack(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
        
    }

    private bool attacked;
    private bool vfxPlayed;
    private Vector2 mouseVec;
    private Vector3 attackPos;
    private Quaternion attackRotation;

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        if (stateDuration > BasicAttackProfile.WindupDuration + BasicAttackProfile.FollowThroughDuration)
        {
            c = GetState<ProtagRunning>();
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        attackPos = playerBody.position + Vector3.up;
            
        mouseVec = 
            Camera.main.ScreenToWorldPoint(inputState.mousePosition) -
            attackPos;
        
        attackRotation = mouseVec.GetAngle();
        
        animator.SetFacing(mouseVec);
        animator.PlayAnimation("Idle");
        heightBody.gravityEnabled = false;
        attacked = false;
        vfxPlayed = false;
        
        transitions.SubOnHitDoDie();
    }

    public override void ExitState()
    {
        heightBody.gravityEnabled = true;
        
        transitions.UnsubOnHitDoDie();
    }

    public override void UpdateState()
    {
        if (!attacked && stateDuration > BasicAttackProfile.WindupDuration)
        {
            var attackProfile = blackboard.basicAttackProfile;
            
            // Attack physics
            bool succeess = blackboard.askPerformAttack.CallFunc(
                attackProfile,
                new AttackInfo
                {
                    attackSourcePosition = attackPos,
                    attackAngle =  attackRotation,
                }
            );
            
            if(succeess)
                blackboard.askFreezeFrame.RaiseEvent(blackboard.basicAttackProfile.FreezeFrameDuration);
            
            attacked = true;
        }

        if (!vfxPlayed && stateDuration > BasicAttackProfile.PlayVFXTime)
        {
            vfxPlayed = true;
            blackboard.askPlayVFX.CallFunc(
                blackboard.basicAttackVFX,
                0,
                new PlayVFXSettings()
                {
                    position = attackPos,
                    rotation = attackRotation
                });

        }
    }

    public override void FixedUpdateState()
    {
        protagMover.SimpleHorizontalMovement(
            Vector2.zero,
            basicMovementProfile.MaxWalkSpeed,
            basicMovementProfile.WalkAcceleration,
            basicMovementProfile.FrictionAcceleration / 2f,
            Time.fixedDeltaTime
            );
    }
}
