using UnityEngine;

namespace MushiSimpleFSM
{
    /// <summary>
    /// Instanced transition table for reusable transition logic
    /// </summary>
    /// <typeparam name="BlkBoard"></typeparam>
    public class TransitionTable<BlkBoard>
    {
        //public delegate bool Transition(ref State<BlkBoard> current);
        
        protected StateMachine<BlkBoard> context;
        protected BlkBoard blackboard => context.Blackboard;

        protected T GetState<T>() where T : GenericState<BlkBoard>
        {
            return context.GetState<T>();
        }
        
        public TransitionTable(StateMachine<BlkBoard> context)
        {
            this.context = context;
        }
    }
}

