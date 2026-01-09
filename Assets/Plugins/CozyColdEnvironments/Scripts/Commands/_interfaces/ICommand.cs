#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Immutable;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommand
    {
        bool IsReadyToExecute { get; }
        bool IsCancelled { get; }
        bool IsSingle { get; }
        string CommandName { get; }

        [Obsolete]
        ImmutableArray<CommandInfo> UndoCommandsOnAdd { get; }

        [Obsolete]
        ImmutableArray<CommandInfo> CancelledByCommands { get; }

        void Execute();

        void Undo();

        CommandInfo GetCommandInfo();
    }
}