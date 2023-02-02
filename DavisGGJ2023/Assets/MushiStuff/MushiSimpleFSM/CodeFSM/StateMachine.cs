using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MushiSimpleFSM
{
    public abstract class StateMachine<BlkBoard>
    {
        // Fields
        protected GenericState<BlkBoard> currentState;
        private BlkBoard blackboardInstance;
        
        private Dictionary<System.Type, GenericState<BlkBoard>> statePool = new();
        private Dictionary<System.Type, TransitionTable<BlkBoard>> transitionTablePool = new();

        // Properties
        public GenericState<BlkBoard> CurrentState => currentState;
        public BlkBoard Blackboard => blackboardInstance;

        public StateMachine(BlkBoard blackboardInstance)
        {
            this.blackboardInstance = blackboardInstance;
            InitializeStatePool();
            InitializeTransitionPool();
        }

        protected abstract void InitializeStatePool();

        /// <summary>
        /// Add to the state pool, using the parameter's explicit type as key
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void AddToStatePool<T>(T instance) where T : GenericState<BlkBoard>
        {
            statePool.TryAdd(typeof(T), instance);
        }
        
        /// <summary>
        ///  Add to the state pool, using the parameter's instance type as key
        /// </summary>
        /// <param name="instance"></param>
        public void AddToStatePoolDynamic(GenericState<BlkBoard> instance) 
        {
            statePool.TryAdd(instance.GetType(), instance);
        }
        
        /// <summary>
        /// Takes a parent state type and finds derived state types in the same assembly.
        /// Intended to work with <see cref="AddTypesToStatePool"/>
        /// </summary>
        /// <param name="constructorArgs"></param>
        /// <typeparam name="ParentType"></typeparam>
        protected static Type[] FindDerivedStateTypes<ParentType>(params object[] constructorArgs) where ParentType : GenericState<BlkBoard>
        {
            var parentType = typeof(ParentType);
            return Assembly.GetAssembly(parentType).GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(parentType))
                .ToArray();
        }
        
        /// <summary>
        /// Instantiates objects from a type array and adds to state pool. Must be matching State types.
        /// Intended to be used with <see cref="FindDerivedStateTypes{ParentType}"/>
        /// </summary>
        /// <param name="types"></param>
        /// <param name="constructorArgs"></param>
        protected void AddTypesToStatePool(Type[] types, params object[] constructorArgs) 
        {
            foreach (Type type in types)
            {
                AddToStatePoolDynamic((GenericState<BlkBoard>)Activator.CreateInstance(type, constructorArgs));
            }
        }
        
        protected abstract void InitializeTransitionPool();

        /// <summary>
        /// Add a new transition table to the pool
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void AddToTransitionPool<T>(T instance) where T : TransitionTable<BlkBoard>
        {
            transitionTablePool.TryAdd(typeof(T), instance);
        }

        public void InitializeEntryState<EntryState>() where EntryState : GenericState<BlkBoard>
        {
            currentState = GetState<EntryState>();
            currentState.stateEntryTime = Time.time;
            currentState.EnterState();
        }

        public void DisableStateMachine()
        {
            currentState.ExitState();
            currentState = null;
        }

        public virtual void Update()
        {
            GenericState<BlkBoard> currentTransition = null;
            if (currentState.TryTransition(ref currentTransition))
            {
                SwitchStates(currentTransition);
            }
            currentState.UpdateState();
        }
        
        public virtual void FixedUpdate()
        {
            currentState.FixedUpdateState();
        }

        /// <summary>
        /// Force a transition outside of the standard frame-based transition checks
        /// </summary>
        /// <param name="state"></param>
        public void ForceTransition(GenericState<BlkBoard> state)
        {
            SwitchStates(state);
        }

        private void SwitchStates(GenericState<BlkBoard> state)
        {
            if (currentState == state || state == null) return;
            currentState.ExitState();
            currentState = state;
            currentState.stateEntryTime = Time.time;
            currentState.EnterState();
        }

        /// <summary>
        /// Get the state instance of some type from this state machine's table pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetState<T>() where T : GenericState<BlkBoard>
        {
            var type = typeof(T);
            if (statePool.TryGetValue(type, out GenericState<BlkBoard> state))
            {
                return (T)state;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Get the transition table of some type from this state machine's table pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetTransitionTable<T>() where T : TransitionTable<BlkBoard>
        {
            var type = typeof(T);
            if (transitionTablePool.TryGetValue(type, out TransitionTable<BlkBoard> transitionTable))
            {
                return (T)transitionTable;
            }
            else
            {
                return null;
            }
        }
    }
}

