using CommunityToolkit.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public class AnonymousCommandBuilder
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

        public AnonymousCommandBuilder()
        {
            Reset();
        }

        public AnonymousCommandBuilder OnExecute(Func<CancellationToken, ValueTask> onExecute)
        {
            Guard.IsNotNull(onExecute, nameof(onExecute));
            ThrowIfStatedCommand();

            this.onExecute = onExecute;

            return this;
        }

        public AnonymousCommandBuilder OnExecute<T>(T state, Func<T, CancellationToken, ValueTask> onExecute)
        {
            Guard.IsNotNull(onExecute, nameof(onExecute));

            this.onExecute = onExecute;

            isStatedCommand = true;

            return this;
        }

        public AnonymousCommandBuilder ExecuteWhen(Func<bool>? executePredicate)
        {
            ThrowIfStatedCommand();

            this.executePredicate = executePredicate;

            return this;
        }

        public AnonymousCommandBuilder ExecuteWhen<T>(T state, Func<T, bool>? executePredicate)
        {
            this.executePredicate = executePredicate;

            isStatedCommand = true;

            return this;
        }

        public AnonymousCommandBuilder OnReset(Action? onReset)
        {
            ThrowIfStatedCommand();

            this.onReset = onReset;

            return this;
        }

        public AnonymousCommandBuilder OnReset<T>(Action<T>? onReset)
        {
            this.onReset = onReset;

            isStatedCommand = true;

            return this;
        }

        public AnonymousCommandBuilder Name(string? name = null)
        {
            this.name = name;

            return this;
        }

        public AnonymousCommandBuilder IsSingleCommand(bool state = true)
        {
            isSingle = state;

            return this;
        }

        public AnonymousCommandBuilder IsResetable(bool state = true)
        {
            isResetable = state; 

            return this; 
        }

        public AnonymousCommandBuilder DelayFrames(int count)
        {
            delayFrameCount = count;

            return this;
        }

        public Command Build()
        {
            ThrowIfOnExecuteIsNull();
            ThrowIfStatedCommand();

            var cmd = new AnonymousCommand(
                (Func<CancellationToken, ValueTask>)onExecute!,
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

        public Command Build<T>(T state)
        {
            ThrowIfOnExecuteIsNull();

            var cmd = new AnonymousCommand<T>(
                state,
                (Func<T, CancellationToken, ValueTask>)onExecute!,
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

        public AnonymousCommandBuilder Reset()
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

        private void ThrowIfStatedCommand()
        {
            if (isStatedCommand)
                throw new InvalidOperationException($"Cannot create a stated command without the state");
        }

        private void ThrowIfOnExecuteIsNull()
        {
            if (onExecute is null)
                throw new InvalidOperationException($"Cannot create a command without {nameof(onExecute)} action");
        }
    }
}
