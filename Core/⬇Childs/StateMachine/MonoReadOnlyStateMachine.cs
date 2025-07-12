using System;
using UTIRLib.Patterns.State;

#nullable enable

namespace UTIRLib.Patterns.StateMachine
{
    public abstract class MonoReadOnlyStateMachine : MonoX, IReadOnlyStateMachine
    {
        protected IStateMachine stateMachineInternal = null!;

        public IReadOnlyState PlayingState => stateMachineInternal.PlayingState;
        public bool IsExecuting => stateMachineInternal.IsExecuting;
        public bool IsIdle => stateMachineInternal.IsIdle;
        public bool IsEnabled { get; set; } = false;

        protected override void OnStart()
        {
            base.OnStart();
            stateMachineInternal = GetStateMachine(GetIdleState(), GetStates(), GetTransitions());
            IsEnabled = true;
        }

        public void Execute() => stateMachineInternal.Execute();

        public void SwitchTo(Type stateType) => stateMachineInternal.SwitchTo(stateType);

        public void SwitchTo(IReadOnlyState state) => stateMachineInternal.SwitchTo(state);

        public IReadOnlyState GetState(Type type) => stateMachineInternal.GetState(type);

        public TState GetState<TState>() => stateMachineInternal.GetState<TState>();

        public bool IsCompleted(Type type) => stateMachineInternal.IsCompleted(type);

        public bool Contains(Type? type) => stateMachineInternal.Contains(type);

        public bool Contains(IReadOnlyState? state) => stateMachineInternal.Contains(state);

        public bool Contains(StateTransition transition) => stateMachineInternal.Contains(transition);

        public bool IsStateExecuting(Type? type) => stateMachineInternal.IsStateExecuting(type);

        public bool IsStateExecuting(IReadOnlyState? state) => stateMachineInternal.IsStateExecuting(state);

        protected void OnUpdate()
        {
            if (IsEnabled && stateMachineInternal.PlayingState.InNormalUpdate)
            {
                stateMachineInternal.Execute();
            }
        }

        protected void OnFixedUpdate()
        {
            if (IsEnabled && stateMachineInternal.PlayingState.InFixedUpdate)
            {
                stateMachineInternal.Execute();
            }
        }

        protected void OnLateUpdate()
        {
            if (IsEnabled && stateMachineInternal.PlayingState.InLateUpdate)
            {
                stateMachineInternal.Execute();
            }
        }

        protected virtual IStateMachine GetStateMachine(IReadOnlyState idleState, IReadOnlyState[] states,
            StateTransition[] transitions) => new StateMachine(idleState, states, transitions);

        protected abstract IReadOnlyState GetIdleState();

        protected abstract IReadOnlyState[] GetStates();

        protected abstract StateTransition[] GetTransitions();
    }
}