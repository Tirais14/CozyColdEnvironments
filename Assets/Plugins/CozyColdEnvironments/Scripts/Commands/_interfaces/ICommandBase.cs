#nullable enable

using R3;
using System;

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

        void Undo();

        void Cancel();

        bool TryReset();

        CommandInfo GetCommandInfo();

        Observable<CommandStatus> ObserveIsDone();
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