#nullable enable

using System;

namespace UTIRLib.Patterns.State
{
    public interface IState : IReadOnlyState
    {
        void AddTransition(IState state);

        void AddTransition(Type stateType);

        void AddTransitions(params IState[] states);

        void AddTransitions(params Type[] stateTypes);

        void RemoveTransition(IState? state);

        void RemoveTransition(Type? stateType);

        void RemoveTransitions(params IState[] states);

        void RemoveTransitions(params Type[] stateTypes);
    }
}