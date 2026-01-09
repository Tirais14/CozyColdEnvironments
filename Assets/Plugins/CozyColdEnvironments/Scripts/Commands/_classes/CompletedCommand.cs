#nullable enable
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    internal class CompletedCommand : ICommand
    {
        public bool IsReadyToExecute { get; } = true;
        public bool IsCancelled { get; }
        public bool IsSingle { get; } = false;
        public bool IsCompleted { get; } = true;
        public bool IsRunning { get; } = false;
        public bool IsDone { get; } = true;
        public bool IsFaulted { get; } = false;
        public string CommandName { get; } = "Completed";

        public void Execute()
        {
        }

        public void Undo()
        {
        }

        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(typeof(CompletedCommand), CommandName);
        }

        public override string ToString()
        {
            return CommandName;
        }
    }
}
