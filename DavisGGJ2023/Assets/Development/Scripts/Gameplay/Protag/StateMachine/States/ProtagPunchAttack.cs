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
            Vector2 mouseVec = 
                Camera.main.ScreenToWorldPoint(inputState.mousePosition) -
                playerBody.transform.position;
            
            blackboard.askPlayVFX.CallFunc(
                blackboard.basicAttackVFX,
                0,
                new PlayVFXSettings()
                {
                    position = playerBody.position,
                    rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(mouseVec.y, mouseVec.x))
                });
            // Attack or whtaever
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
