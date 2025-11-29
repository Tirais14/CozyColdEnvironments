#nullable enable
using CommunityToolkit.Diagnostics;
using System;

namespace CCEnvs.Patterns.Commands
{
    public partial class AnonymousCommand : Command, ICommand
    {
        private readonly Func<bool> isReadyToExecute;
        private readonly Action execute;

        public override bool IsReadyToExecute => isReadyToExecute();

        public AnonymousCommand(Func<bool> isReadyToExecute,
            Action execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
            :
            base(name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand)
        {
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override void Execute() => execute();

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}";
        }
    }
    public class AnonymousCommand<T> : Command, ICommand
    {
        private readonly T state;
        private readonly Predicate<T> isReadyToExecute;
        private readonly Action<T> execute;

        public override bool IsReadyToExecute => isReadyToExecute(state);

        public AnonymousCommand(T state,
            Predicate<T> isReadyToExecute,
            Action<T> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
            :
            base(name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand)
        {
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.state = state;
            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override void Execute() => execute(state);

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}; {nameof(state)}: {state}";
        }
    }
    public class AnonymousCommand<T, T1> : Command, ICommand
    {
        private readonly T state;
        private readonly T1 state1;
        private readonly Func<T, T1, bool> isReadyToExecute;
        private readonly Action<T, T1> execute;

        public override bool IsReadyToExecute => isReadyToExecute(state, state1);

        public AnonymousCommand(T state,
            T1 state1,
            Func<T, T1, bool> isReadyToExecute,
            Action<T, T1> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
            :
            base(name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand)
        {
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.state = state;
            this.state1 = state1;
            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override void Execute() => execute(state, state1);

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}; {nameof(state)}: {state}; {nameof(state1)}: {state1}";
        }
    }
    public class AnonymousCommand<T, T1, T2> : Command, ICommand
    {
        private readonly T state;
        private readonly T1 state1;
        private readonly T2 state2;
        private readonly Func<T, T1, T2, bool> isReadyToExecute;
        private readonly Action<T, T1, T2> execute;

        public override bool IsReadyToExecute => isReadyToExecute(state, state1, state2);

        public AnonymousCommand(T state,
            T1 state1,
            T2 state2,
            Func<T, T1, T2, bool> isReadyToExecute,
            Action<T, T1, T2> execute,
            string? name = null,
            CommandInfo[]? undoCommandsOnAdd = null,
            bool singleCommand = false)
            :
            base(name: name,
                undoCommandsOnAdd: undoCommandsOnAdd,
                singleCommand: singleCommand)
        {
            Guard.IsNotNull(isReadyToExecute);
            Guard.IsNotNull(execute);

            this.state = state;
            this.state1 = state1;
            this.state2 = state2;
            this.isReadyToExecute = isReadyToExecute;
            this.execute = execute;
        }

        public override void Execute() => execute(state, state1, state2);

        public override string ToString()
        {
            return $"{nameof(CommandName)}: {CommandName}; {nameof(state)}: {state}; {nameof(state1)}: {state1}; {nameof(state2)}: {state2}";
        }
    }
}
