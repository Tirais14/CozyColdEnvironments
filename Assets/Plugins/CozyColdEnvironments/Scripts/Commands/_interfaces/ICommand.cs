#nullable enable

using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommand
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
        int DelayFrameCount { get; }

        void Execute();

        void Undo();

        ICommand Reset();

        bool TryReset();

        CommandInfo GetCommandInfo();
    }
}