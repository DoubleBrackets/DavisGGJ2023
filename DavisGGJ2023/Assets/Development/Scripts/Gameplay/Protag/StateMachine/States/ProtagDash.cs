using MushiSimpleFSM;
using UnityEngine;

public class ProtagDash : ProtagState
{
    public ProtagDash(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    private Vector2 mouseVec;

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        
        return false;
    }

    public override void EnterState()
    {
        heightBody.gravityEnabled = false;
        
        var pos = playerBody.position + Vector3.up;
        mouseVec = 
            Camera.main.ScreenToWorldPoint(inputState.mousePosition) -
            pos;

        mouseVec.Normalize();
        
        /*animator.SetFacing(mouseVec);
        animator.PlayAnimation("Dash");*/
    }

    public override void ExitState()
    {
        //heightBody.gravityEnabled = true;
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        /*protagMover.SimpleHorizontalMovement(
            mouseVec.normalized,
            basicMovementProfile.
            10000f,
            basicMovementProfile.FrictionAcceleration,
            Time.fixedDeltaTime
        );*/
        
    }
}
