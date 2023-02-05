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

    public void SubBasicActions()
    {
        SubOnDashTryWarp();
        SubOnPrimaryDoBasicAttack();
        SubOnHitDoStaggered();
        SubOnHazardDoDie();
    }
    
    public void UnsubBasicActions()
    {
        UnsubOnDashTryWarp();
        UnsubOnPrimaryDoBasicAttack();
        UnsubOnHitDoStaggered();
    }

    public void SubOnPrimaryDoBasicAttack()
    {
        blackboard.InputProvider.Events.OnPrimaryFirePressed += TryToBasicAttack;
    }

    private void TryToBasicAttack()
    {
        if(Time.time - blackboard.basicAttackFinishTime > blackboard.basicAttackProfile.Cooldown)
            context.ForceTransition(GetState<ProtagPunchAttack>());
    }
    
    public void UnsubOnPrimaryDoBasicAttack()
    {
        blackboard.InputProvider.Events.OnPrimaryFirePressed -= TryToBasicAttack;
    }
    
    #region Root Warping
    public void SubOnDashTryWarp()
    {
        blackboard.InputProvider.Events.OnDashPressed += TryToStartRootWarp;
        blackboard.askPerformRootWarp.OnRaised += PerformRootWarp;
    }

    private void TryToStartRootWarp()
    {
        // Attempt the attack
        bool successful = blackboard.askPerformAttack.CallFunc(
            blackboard.tryRootWarpAttackProfile,
            new AttackInfo
            {
                attackSourcePositionRaw = blackboard.playerBodyTransform.position + Vector3.up
            }
        );
    }

    private void PerformRootWarp(Vector3 position, float travelTime)
    {
        blackboard.warpTarget = position;
        blackboard.warpTravelTime = travelTime;
        context.ForceTransition(GetState<ProtagWarping>());
    }
    
    public void UnsubOnDashTryWarp()
    {
        blackboard.InputProvider.Events.OnDashPressed -= TryToStartRootWarp;
        blackboard.askPerformRootWarp.OnRaised -= PerformRootWarp;
    }
    #endregion
    
    #region Combat Hits
    public void SubOnHitDoStaggered()
    {
        blackboard.protagCombatEntity.onAttackReceived += GetHit;
    }
    private bool GetHit(AttackProfileSO attackProfileSo, AttackInfo attackInfo)
    {
        if (attackProfileSo.Damage != 0)
        {
            HazardHit();
        }
        else
        {
            blackboard.recentlyHit = attackProfileSo;
            blackboard.recentlyHitInfo = attackInfo;
            context.ForceTransition(GetState<ProtagStaggered>());
        }
        return true;
    }
    
    public void UnsubOnHitDoStaggered()
    {
        blackboard.protagCombatEntity.onAttackReceived -= GetHit;
    }
    
    public void SubOnHazardDoDie()
    {
        blackboard.heightBody.onHitHazard += HazardHit;
    }
    
    private void HazardHit()
    {
        context.ForceTransition(GetState<ProtagDead>());
    }
    

    public void UnsubOnHazardDoDie()
    {
        blackboard.heightBody.onHitHazard -= HazardHit;
    }
    #endregion
}
