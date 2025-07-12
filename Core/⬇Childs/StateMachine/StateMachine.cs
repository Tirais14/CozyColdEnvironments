#nullable enable

using System;
using UTIRLib.Collections;
using UTIRLib.Extensions;
using UTIRLib.Patterns.State;

namespace UTIRLib.Patterns.StateMachine
{
    public class StateMachine : ReadOnlyStateMachine, IStateMachine
    {
        public StateMachine(IReadOnlyState idleState, IReadOnlyState[] states, StateTransition[] transitions)
            : base(idleState, states, transitions)
        {
        }

        public void AddState(IReadOnlyState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            states.Add(state.GetType(), state);
        }

        public void AddStates(params IReadOnlyState[] states) => states.ForEach(state => AddState(state));

        public void AddTransition(StateTransition transition) => transitions.Add(transition);

        public void AddTransitions(params StateTransition[] transitions) => this.transitions.AddRange(transitions);

        public void RemoveState(IReadOnlyState? state)
        {
            if (state == null) return;

            states.Remove(state.GetType());
        }

        public void RemoveStates(params IReadOnlyState?[] states) => states.ForEach(state => RemoveState(state));

        public void RemoveTransition(StateTransition transition) => transitions.Remove(transition);

        public void RemoveTransitions(params StateTransition[] transitions)
        {
            for (int i = 0; i < transitions.Length; i++)
            {
                RemoveTransition(transitions[i]);
            }
        }
    }
}