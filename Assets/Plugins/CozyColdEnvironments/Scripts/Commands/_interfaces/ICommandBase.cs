#nullable enable

using R3;
using System;
using System.Threading;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandBase : IDisposable
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        bool IsSingle { get; }
        bool IsCompleted { get; }
        bool IsRunning { get; }
        bool IsDone { get; }
        bool IsFaulted { get; }
        bool IsResetable { get; }
        bool IsValid { get; }

        string Name { get; }

        int DelayFrameCount { get; set; }

        CommandStatus Status { get; }

        Type CommandType { get; }

        CancellationToken CancellationToken { get; }

        CommandSignature Signature { get; }

        void Undo();

        void Cancel();

        bool TryReset();

        [Obsolete]
        CommandSignature GetCommandSignature();

        IDisposable GetCancellationHandle();

        Observable<CommandStatus> ObserveIsDone();
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