#nullable enable
using CCEnvs.Returnables;
using System;
using System.Threading;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler
    {
        bool HasCommands { get; }

        void AddCommand(ICommand command);

        void Clear();

        void DoTick();

        IObservable<ICommand> ObserveAddCommand();

        IObservable<Mock> ObserveCommandsExecuted();
    }
}
