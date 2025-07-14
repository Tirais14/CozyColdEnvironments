#nullable enable
using System;

namespace UTIRLib.Patterns.States
{
    /// <summary>
    /// Realize this to set switch conditions
    /// </summary>
    public interface IStateMachineSwitchStrategy<T>
        where T : IStateBase
    {
        T PreviousState { get; }
        T SelectedState { get; }
        T DefaultState { get; }

        T GetNextState();

        void ForceSelectState(T state);

        void ResetState();
    }
}
