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
        transitions.SubOnPrimaryDoBasicAttack();
        transitions.SubOnHitDoDie();
    }

    public override void ExitState()
    {
        transitions.UnsubOnPrimaryDoBasicAttack();
        transitions.UnsubOnHitDoDie();
    }

    public override void UpdateState()
    {
        animator.SetFacing(inputState.movementVector);
        animator.PlayAnimation(heightBody.horizontalVel.magnitude > 0.1f ? "Run" : "Idle");
    }

    public override void FixedUpdateState()
    {
        float accelT = Mathf.InverseLerp(0f,basicMovementProfile.MaxWalkSpeed, heightBody.horizontalVel.magnitude);
        float accel = basicMovementProfile.WalkAcceleration *
                      Mathf.Clamp01(basicMovementProfile.AccelerationCurve.Evaluate(accelT));
        
        protagMover.SimpleHorizontalMovement(
            inputState.movementVector,
            basicMovementProfile.MaxWalkSpeed,
            accel,
            basicMovementProfile.FrictionAcceleration,
            Time.fixedDeltaTime
        );
        
    }
}
