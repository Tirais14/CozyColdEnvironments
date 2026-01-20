using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public struct CommandBuilder
    {
        private Delegate? onExecute;
        private Delegate? executePredicate;
        private Delegate? onReset;
        private string? name;
        private bool isSingle;
        private bool isResetable;
        private int delayFrameCount;

        private bool isStatedCommand;

        public event Action<ICommand>? OnBuilded;

        public static CommandBuilder Create()
        {
            return new CommandBuilder().Reset();
        }

        public CommandBuilder OnExecute(Func<CancellationToken, ValueTask>? onExecute)
        {
            ThrowIfStatedCommand();

            this.onExecute = onExecute;

            return this;
        }

        public CommandBuilder OnExecute<T>(T state, Func<T, CancellationToken, ValueTask>? onExecute)
        {
            this.onExecute = onExecute;

            isStatedCommand = true;

            return this;
        }

        public CommandBuilder ExecuteWhen(Func<bool>? executePredicate)
        {
            ThrowIfStatedCommand();

            this.executePredicate = executePredicate;

            return this;
        }

        public CommandBuilder ExecuteWhen<T>(T state, Func<T, bool>? executePredicate)
        {
            this.executePredicate = executePredicate;

            isStatedCommand = true;

            return this;
        }

        public CommandBuilder OnReset(Action? onReset)
        {
            ThrowIfStatedCommand();

            this.onReset = onReset;

            return this;
        }

        public CommandBuilder OnReset<T>(Action<T>? onReset)
        {
            this.onReset = onReset;

            isStatedCommand = true;

            return this;
        }

        public CommandBuilder Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        public CommandBuilder IsSingleCommand(bool state = true)
        {
            isSingle = state;

            return this;
        }

        public CommandBuilder IsResetable(bool state = true)
        {
            isResetable = state; 

            return this; 
        }

        public CommandBuilder DelayFrames(int count)
        {
            delayFrameCount = count;

            return this;
        }

        public readonly Command Build()
        {
            ThrowIfStatedCommand();

            var cmd = new AnonymousCommand(
                (Func<CancellationToken, ValueTask>?)onExecute,
                isReadyToExecute: (Func<bool>?)executePredicate,
                onReset: (Action?)onReset,
                name: name,
                isSingle: isSingle,
                isResetable: isResetable,
                delayFrameCount: delayFrameCount
                );

            OnBuilded?.Invoke(cmd);

            return cmd;
        }

        public readonly Command Build<T>(T state)
        {
            var cmd = new AnonymousCommand<T>(
                state,
                (Func<T, CancellationToken, ValueTask>?)onExecute!,
                isReadyToExecute: (Func<T, bool>?)executePredicate,
                onReset: (Action<T>?)onReset,
                name: name,
                isSingle: isSingle,
                isResetable: isResetable,
                delayFrameCount: delayFrameCount
                );

            OnBuilded?.Invoke(cmd);

            return cmd;
        }

        public CommandBuilder Reset()
        {
            onExecute = null;
            executePredicate = null;
            onReset = null;
            name = null;
            isSingle = false;
            isResetable = true;
            delayFrameCount = 0;

            isStatedCommand = false;

            return this;
        }

        private readonly void ThrowIfStatedCommand()
        {
            if (isStatedCommand)
                throw new InvalidOperationException($"Cannot create a stated command without the state");
        }
    }
}
