#nullable enable
using CCEnvs.Returnables;
using System;
using System.Threading;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler
    {
        /// <summary>
        /// Single-use. After commands executed old token source is disposed, and creates new one.
        /// </summary>
        CancellationToken CommandsExecutedCancellationToken { get; }
        bool HasCommands { get; }

        void AddCommand(ICommand command);

        void Clear();

        void DoTick();

        IObservable<ICommand> ObserveAddCommand();

        IObservable<Mock> ObserveCommandsExecuted();
    }
}
