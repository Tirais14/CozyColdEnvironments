#nullable enable

using R3;
using System;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommand : IDisposable
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        bool IsSingle { get; }
        bool IsCompleted { get; }
        bool IsRunning { get; }
        bool IsDone { get; }
        bool IsFaulted { get; }
        bool IsResetable { get; }
        //bool IsAsync { get; }
        string Name { get; }
        int DelayFrameCount { get; set; }
        CommandStatus Status { get; }

        //void Execute();

        ValueTask ExecuteAsync();

        void Undo();

        ICommand Reset();

        bool TryReset();

        CommandInfo GetCommandInfo();

        Observable<CommandStatus> ObserveIsDone();
    }

    public static class ICommandExtensions
    {
        public static ICommand ScheduleBy(this ICommand command, ICommandScheduler commandScheduler)
        {
            CC.Guard.IsNotNull(command, nameof(command));
            CC.Guard.IsNotNull(commandScheduler, nameof(commandScheduler));

            commandScheduler.Schedule(command);

            return command;
        }
    }
}