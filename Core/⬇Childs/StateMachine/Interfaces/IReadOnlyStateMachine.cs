using System;
using UTIRLib.Patterns.State;

#nullable enable

namespace UTIRLib.Patterns.StateMachine
{
    public interface IReadOnlyStateMachine : IExecutable
    {
        bool IsIdle { get; }
        IReadOnlyState PlayingState { get; }

        void SwitchTo(Type stateType);

        void SwitchTo(IReadOnlyState state);

        IReadOnlyState GetState(Type type);

        TState GetState<TState>();

        bool Contains(IReadOnlyState? state);

        bool Contains(Type? type);

        bool Contains(StateTransition transition);

        bool IsCompleted(Type type);

        bool IsStateExecuting(IReadOnlyState? state);

        bool IsStateExecuting(Type? type);
    }
}