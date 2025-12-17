#nullable enable
using System.Collections.Immutable;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommand
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        bool IsSingle { get; }
        string CommandName { get; }
        ImmutableArray<CommandInfo> UndoCommandsOnAdd { get; }
        ImmutableArray<CommandInfo> CancelledByCommands { get; }

        void Execute();

        void Undo();

        CommandInfo GetCommandInfo();
    }
}