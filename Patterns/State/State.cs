using System;
using System.Collections.Generic;
using UTIRLib.Patterns.StateMachine;

#nullable enable

namespace UTIRLib.Patterns.State
{
    public abstract class State : ReadOnlyState, IState
    {
        protected State(IEnumerable<Type> transitions, UpdateMethod updateAttributes = UpdateMethod.Normal,
            bool isAbortable = false) : base(transitions, updateAttributes, isAbortable)
        { }

        protected State(StateParameters stateParameters) : this(stateParameters.transitions,
            stateParameters.updateAttributes, stateParameters.isAbortable)
        { }

        public void AddTransition(IState state) => AddTransition(state.GetType());

        public void AddTransition(Type stateType)
        {
            if (stateType == null)
            {
                throw new ArgumentNullException(nameof(stateType));
            }

            transitions.Add(stateType);
        }

        public void AddTransitions(params IState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                AddTransition(states[i]);
            }
        }

        public void AddTransitions(params Type[] stateTypes)
        {
            for (int i = 0; i < stateTypes.Length; i++)
            {
                AddTransition(stateTypes[i]);
            }
        }

        public void RemoveTransition(IState? state) => RemoveTransition(state?.GetType());

        public void RemoveTransition(Type? stateType)
        {
            if (stateType == null) return;

            transitions.Remove(stateType);
        }

        public void RemoveTransitions(params IState[] states)
        {
            for (int i = 0; i < states.Length; i++)
            {
                RemoveTransition(states[i]);
            }
        }

        public void RemoveTransitions(params Type[] stateTypes)
        {
            for (int i = 0; i < stateTypes.Length; i++)
            {
                RemoveTransition(stateTypes[i]);
            }
        }
    }
}