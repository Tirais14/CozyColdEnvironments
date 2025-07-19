#nullable enable
using System;

namespace UTIRLib.Patterns.States
{
    public interface IStateMachine : IExecutable
    {
        Type PlayingStateType { get; }

        void PlayPreviousState();

        void PlayDefaultState();
    }
    public interface IStateMachine<T> : IStateMachine
        where T : IStateBase
    {
        void PlayState(T state);
    }
}
