#nullable enable

using System;
using System.Threading;
using R3;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandBase : IDisposable
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        bool IsSingle { get; set; }
        bool IsCompleted { get; }
        bool IsRunning { get; }
        bool IsDone { get; }
        bool IsFaulted { get; }
        bool IsResetable { get; }
        bool IsValid { get; }

        string Name { get; set; }

        CommandStatus Status { get; }

        Type CommandType { get; }

        CommandSignature Signature { get; }

        void Undo();

        void Cancel();

        bool TryReset();

        IDisposable GetCancellationHandle();

        Observable<CommandStatus> ObserveIsDone();

        Observable<CommandStatus> ObserveStatus();
    }

    public interface ICommandBase<TThis> : ICommandBase
        where TThis : ICommandBase
    {
        TThis Reset();

        TThis AttachExternalCancellationToken(CancellationToken cancellationToken);
    }

    public static class ICommandBaseExtensions
    {
        public static T ScheduleBy<T>(this T command, ICommandScheduler commandScheduler)
            where T : ICommandBase
        {
            CC.Guard.IsNotNull(command, nameof(command));
            CC.Guard.IsNotNull(commandScheduler, nameof(commandScheduler));

            commandScheduler.Schedule(command);

            return command;
        }
    }
}