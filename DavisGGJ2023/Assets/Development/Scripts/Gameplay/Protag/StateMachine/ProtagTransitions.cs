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
            state = GetState<ProtagWalking>();
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
}
