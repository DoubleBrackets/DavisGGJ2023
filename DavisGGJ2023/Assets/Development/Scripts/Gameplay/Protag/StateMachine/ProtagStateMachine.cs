using System;
using MushiSimpleFSM;
using UnityEngine;

public class ProtagStateMachine : StateMachine<ProtagBlackboard>
{
    private static Type[] stateTypes;
    
    public ProtagStateMachine(ProtagBlackboard blackboardInstance) : base(blackboardInstance)
    {
    }

    protected override void InitializeStatePool()
    {
        if (stateTypes == null)
        {
            stateTypes = FindDerivedStateTypes<ProtagState>();
        }
        AddTypesToStatePool(stateTypes, this);
    }

    protected override void InitializeTransitionPool()
    {
        AddToTransitionPool(new ProtagTransitions(this));
    }
}
