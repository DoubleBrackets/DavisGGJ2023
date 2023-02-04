using MushiSimpleFSM;
using UnityEngine;

public class ProtagTransitions : TransitionTable<ProtagBlackboard>
{
    public ProtagTransitions(StateMachine<ProtagBlackboard> context) : base(context)
    {
    }

    public bool IdleToWalking(ref GenericState<ProtagBlackboard> state)
    {
        if (blackboard.InputState.movementVector != Vector2.zero)
        {
            state = GetState<ProtagRunning>();
            return true;
        }

        return false;
    }
    
    public bool WalkingToIdle(ref GenericState<ProtagBlackboard> state)
    {
        if (blackboard.InputState.movementVector == Vector2.zero)
        {
            state = GetState<ProtagIdle>();
            return true;
        }

        return false;
    }
    
    public bool GroundedToWalking(ref GenericState<ProtagBlackboard> state)
    {
        if (blackboard.heightBody.isGrounded)
        {
            state = GetState<ProtagRunning>();
            return true;
        }

        return false;
    }
    
    public bool AirborneToFalling(ref GenericState<ProtagBlackboard> state)
    {
        if (!blackboard.heightBody.isGrounded)
        {
            state = GetState<ProtagFalling>();
            return true;
        }

        return false;
    }

    public void SubOnPrimaryDoBasicAttack()
    {
        blackboard.InputProvider.Events.OnPrimaryFirePressed += TryToBasicAttack;
    }

    private void TryToBasicAttack()
    {
        context.ForceTransition(GetState<ProtagPunchAttack>());
    }
    
    public void UnsubOnPrimaryDoBasicAttack()
    {
        blackboard.InputProvider.Events.OnPrimaryFirePressed -= TryToBasicAttack;
    }
    
    public void SubOnHitDoDie()
    {
        blackboard.protagCombatEntity.onAttackReceived += TryToDie;
        blackboard.heightBody.onHitHazard += TryToDie;
    }
    private bool TryToDie(AttackProfileSO attackProfileSo, AttackInfo attackInfo)
    {
        TryToDie();
        return true;
    }

    private void TryToDie()
    {
        context.ForceTransition(GetState<ProtagDead>());
    }
    
    public void UnsubOnHitDoDie()
    {
        blackboard.protagCombatEntity.onAttackReceived -= TryToDie;
        blackboard.heightBody.onHitHazard -= TryToDie;
    }
}
