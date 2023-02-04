using MushiSimpleFSM;
using UnityEngine;

public class ProtagPunchAttack : ProtagState
{
    public ProtagPunchAttack(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
        
    }

    private bool attacked;

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
        Vector2 mouseVec = 
            Camera.main.ScreenToWorldPoint(inputState.mousePosition) -
            playerBody.transform.position;
        
        animator.SetFacing(mouseVec);
        animator.PlayAnimation("Idle");
        attacked = false;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        if (!attacked && stateDuration > BasicAttackProfile.WindupDuration)
        {
            var attackProfile = blackboard.basicAttackProfile;
            Vector3 playerPos = playerBody.position;
            Vector2 mouseVec = 
                Camera.main.ScreenToWorldPoint(inputState.mousePosition) -
                playerPos;

            var attackRotation = mouseVec.GetAngle();
            
            blackboard.askPlayVFX.CallFunc(
                blackboard.basicAttackVFX,
                0,
                new PlayVFXSettings()
                {
                    position = playerPos,
                    rotation = attackRotation
                });
            // Attack physics
            blackboard.askPerformAttack.CallFunc(
                attackProfile,
                new AttackInfo
                {
                    attackSourcePosition = playerPos,
                    attackAngle =  attackRotation,
                }
            );
            
            
            attacked = true;
            blackboard.askFreezeFrame.RaiseEvent(blackboard.basicAttackProfile.FreezeFrameDuration);
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
