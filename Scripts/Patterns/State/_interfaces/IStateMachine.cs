using System;

#nullable enable
namespace CozyColdEnvironments.Patterns.States
{
    public interface IStateMachine
    {
        bool IsExecuting(Type stateType);
        bool IsExecuting(IState state);

        void ForceStopState(Type stateType);
        void ForceStopState(IState state);

        bool IsForceStopable(Type stateType);
        bool IsForceStopable(IState? state);
    }
}
