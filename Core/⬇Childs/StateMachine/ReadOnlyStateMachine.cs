using System;
using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;
using UTIRLib.Patterns.State;

#nullable enable

namespace UTIRLib.Patterns.StateMachine
{
    public class ReadOnlyStateMachine : IReadOnlyStateMachine
    {
        protected readonly IReadOnlyState idleState;
        protected readonly Dictionary<Type, IReadOnlyState> states = new();
        protected readonly List<StateTransition> transitions = new();
        protected int transitionsCount;
        protected IReadOnlyState playingState;

        public IReadOnlyState PlayingState => playingState;
        public bool IsExecuting => playingState != idleState;
        public bool IsIdle => playingState == idleState;

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ReadOnlyStateMachine(IReadOnlyState idleState, IReadOnlyState[] states, StateTransition[] transitions)
        {
            if (idleState is null)
            {
                throw new ArgumentNullException(nameof(idleState));
            }
            else if (states.HasNullElement())
            {
                throw new ArgumentException(nameof(states));
            }
            else if (transitions.IsEmpty())
            {
                Debug.LogWarning(new CollectionArgumentException(nameof(transitions), transitions));
            }

            playingState = idleState;
            this.idleState = idleState;
            this.states.Add(idleState.GetType(), idleState);
            for (int i = 0; i < states.Length; i++)
            {
                this.states.Add(states[i].GetType(), states[i]);
            }
            this.transitions.AddRange(transitions);
            transitionsCount = transitions.Length;
        }

        public void Execute()
        {
            bool isNotSwitched = true;
            bool isPlayingStateCompleted = playingState.IsCompleted;
            bool isPlayingStateAbortable = playingState.IsAbortable;
            StateTransition transition;
            for (int i = 0; i < transitionsCount; i++)
            {
                transition = transitions[i];
                if (playingState.CanSwitchTo(transition.nextStateType) && transition.IsCompleted &&
                    (isPlayingStateCompleted || (isPlayingStateAbortable && (transition.canAbortPrevious || IsIdle))))
                {
                    SwitchTo(transitions[i].nextStateType);
                    isNotSwitched = false;
                }
            }

            if (!IsIdle && isNotSwitched && isPlayingStateCompleted)
            {
                SwitchTo(idleState);
            }

            playingState.Execute();
        }

        public void SwitchTo(Type stateType)
        {
            playingState.Exit();
            playingState = states[stateType];
            playingState.Enter();
        }

        public void SwitchTo(IReadOnlyState state) => SwitchTo(state.GetType());

        public IReadOnlyState GetState(Type type) => states[type];

        public TState GetState<TState>() => GetState(typeof(TState)).Convert<TState>() ??
            throw new KeyNotFoundException(typeof(TState).Name);

        public bool IsCompleted(Type type) => states[type].IsCompleted;

        public bool Contains(Type? type) => type != null && states.ContainsKey(type);

        public bool Contains(IReadOnlyState? state) => Contains(state?.GetType());

        public bool Contains(StateTransition transition) => transitions.Contains(transition);

        public bool IsStateExecuting(Type? type) => playingState != null && playingState.GetType() == type;

        public bool IsStateExecuting(IReadOnlyState? state) => IsStateExecuting(state?.GetType());
    }
}