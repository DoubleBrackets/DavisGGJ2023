using MushiSimpleFSM;
using UnityEngine;

public class ProtagWarping : ProtagState
{
    public ProtagWarping(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    private Vector2 mouseVec;

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        if (stateDuration >= blackboard.warpTravelTime)
        {
            c = GetState<ProtagIdle>();
            return true;
        }
        return false;
    }

    public override void EnterState()
    {
        SetPlayerShown(false);
        heightBody.horizontalVel = Vector2.zero;
        heightBody.verticalVelocity = 0f;
        
        playerBody.transform.position = blackboard.warpTarget + Vector3.up * blackboard.warpTarget.z;
        
        heightBody.height = blackboard.warpTarget.z;
        heightBody.horizontalPos = blackboard.warpTarget;
    }

    public override void ExitState()
    {
       SetPlayerShown(true);
    }

    private void SetPlayerShown(bool val)
    {
        playerBody.gameObject.SetActive(val);
        heightBody.ShadowTransform.gameObject.SetActive(val);
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
        
    }
}
