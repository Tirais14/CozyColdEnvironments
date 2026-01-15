#nullable enable

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
        string CommandName { get; }
        int DelayFrameCount { get; set; }

        ValueTask ExecuteAsync();

        void Undo();

        ICommand Reset();

        bool TryReset();

        CommandInfo GetCommandInfo();
    }
}