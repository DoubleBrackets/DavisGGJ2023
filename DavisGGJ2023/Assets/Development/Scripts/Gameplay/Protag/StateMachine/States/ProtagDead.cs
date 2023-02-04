using MushiSimpleFSM;
using UnityEngine;

public class ProtagDead : ProtagState
{
    public ProtagDead(StateMachine<ProtagBlackboard> stateMachine) : base(stateMachine)
    {
    }

    private bool tryRestart;

    public override bool TryTransition(ref GenericState<ProtagBlackboard> c)
    {
        return false;
    }

    public override void EnterState()
    {
        blackboard.askStopFollowingTarget.RaiseEvent();
        playerBody.gameObject.SetActive(false);
        heightBody.ShadowTransform.gameObject.SetActive(false);
        tryRestart = false;

        blackboard.askPlayVFX.CallFunc(
            blackboard.deathVFX,
            0,
            new PlayVFXSettings
            {
                position = playerBody.position + Vector3.up,
                rotation = Quaternion.identity
            }
        );
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        if (!tryRestart && stateDuration > 2f)
        {
            tryRestart = true;
            blackboard.askRestartLevel.RaiseEvent();
        }
    }

    public override void FixedUpdateState()
    {
        
    }
}
