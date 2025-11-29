#nullable enable
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CCEnvs.Patterns.Commands
{
    public partial class Command 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand Create<T>(
            Func<bool> isReadyToExecute,
            Action execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand(
                isReadyToExecute,
                execute,
                name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T> Create<T>(
            T state,
            Predicate<T> isReadyToExecute,
            Action<T> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand<T>(
                state,
                isReadyToExecute,
                execute,
                name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T, T1> Create<T, T1>(
            T state,
            T1 state1,
            Func<T, T1, bool> isReadyToExecute,
            Action<T, T1> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand<T, T1>(
                state,
                state1,
                isReadyToExecute,
                execute,
                name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AnonymousCommand<T, T1, T2> Create<T, T1, T2>(
            T state,
            T1 state1,
            T2 state2,
            Func<T, T1, T2, bool> isReadyToExecute,
            Action<T, T1, T2> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
        {
            return new AnonymousCommand<T, T1, T2>(
                state,
                state1,
                state2,
                isReadyToExecute,
                execute,
                name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand
                );
        }
    }

    public abstract partial class Command : ICommand
    {
        public abstract bool IsReadyToExecute { get; }
        public bool IsCancelled { get; private set; }
        public string CommandName { get; } = string.Empty;
        public bool IsSingle { get; }
        public ImmutableArray<CommandInfo> UndoCommandsOnAdd { get; private set; } = ImmutableArray<CommandInfo>.Empty;
        public ImmutableArray<CommandInfo> CancelledByCommands { get; private set; } = ImmutableArray<CommandInfo>.Empty;

        public Command()
        {
        }

        public Command(
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
        {
            CommandName = name ?? GetType().ToString();
            UndoCommandsOnAdd = undoCommandsOnAdd?.ToImmutableArray() ?? ImmutableArray<CommandInfo>.Empty;
            IsSingle = singleCommand;
        }

        public abstract void Execute();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Undo() => IsCancelled = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandInfo GetCommandInfo()
        {
            return new CommandInfo(GetType(), CommandName);
        }

        public override string ToString()
        {
            if (CommandName.IsNullOrWhiteSpace())
                return GetType().ToString();
            else
                return CommandName;
        }
    }
}
