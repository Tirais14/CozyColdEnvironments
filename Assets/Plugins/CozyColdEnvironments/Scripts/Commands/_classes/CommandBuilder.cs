using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public struct CommandBuilder
    {
        public string? Name;

        public bool IsSingle;
        public bool IsResetable;

        public int DelayFrameCount;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CommandBuilder Create()
        {
            return new CommandBuilder().Reset();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder SetName(string? name = null)
        {
            this.Name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder AsSingle(bool state = true)
        {
            IsSingle = state;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder AsResetable(bool state = true)
        {
            IsResetable = state; 

            return this; 
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder SetDelayFrames(int count)
        {
            DelayFrameCount = count;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Sync Syncronously() => new(this);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Sync.Stated<TState> Syncronously<TState>(TState state)
        {
            return new Sync.Stated<TState>(state, this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Async Asyncronously() => new(this);

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Async.Stated<TState> Asyncronously<TState>(TState state)
        {
            return new Async.Stated<TState>(state, this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder Reset()
        {
            Name = null;
            IsSingle = false;
            IsResetable = false;
            DelayFrameCount = 0;

            return this;
        }

        public struct Async
        {
            private CommandBuilder builderBase;

            public Func<CancellationToken, ValueTask>? ExecuteAction;
            public Func<bool>? ExecutePredicate;
            public Action? ResetAction;

            public Async(CommandBuilder builderBase)
                :
                this()
            {
                this.builderBase = builderBase;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Async SetExecuteAction(Func<CancellationToken, ValueTask>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Async SetExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }


            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Async SetResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Async Reset()
            {
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommandAsync Build()
            {
                return new AnonymousCommandAsync(
                    onExecute: ExecuteAction,
                    isReadyToExecute: ExecutePredicate,
                    onReset: ResetAction,
                    name: builderBase.Name,
                    isSingle: builderBase.IsSingle,
                    isResetable: builderBase.IsResetable,
                    delayFrameCount: builderBase.DelayFrameCount
                    );
            }

            public struct Stated<TState>
            {
                private CommandBuilder builderBase;

                public TState State;
                public Func<TState, CancellationToken, ValueTask>? ExecuteAction;
                public Func<TState, bool>? ExecutePredicate;
                public Action<TState>? ResetAction;

                public Stated(TState state, CommandBuilder builderBase)
                    :
                    this()
                {
                    State = state;
                    this.builderBase = builderBase;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetExecuteAction(Func<TState, CancellationToken, ValueTask>? executeAction)
                {
                    ExecuteAction = executeAction;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetExecutePredicate(Func<TState, bool>? executePredicate)
                {
                    ExecutePredicate = executePredicate;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetResetAction(Action<TState>? resetAction)
                {
                    ResetAction = resetAction;

                    return this;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> Reset()
                {
                    ExecuteAction = null;
                    ExecutePredicate = null;
                    ResetAction = null;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly AnonymousCommandAsync<TState> Build()
                {
                    return new AnonymousCommandAsync<TState>(
                        state: State,
                        onExecute: ExecuteAction,
                        isReadyToExecute: ExecutePredicate,
                        onReset: ResetAction,
                        name: builderBase.Name,
                        isSingle: builderBase.IsSingle,
                        isResetable: builderBase.IsResetable,
                        delayFrameCount: builderBase.DelayFrameCount
                        );
                }
            }
        }

        public struct Sync
        {
            private CommandBuilder builderBase;

            public Action? ExecuteAction;
            public Func<bool>? ExecutePredicate;
            public Action? ResetAction;

            public Sync(CommandBuilder builderBase)
                :
                this()
            {
                this.builderBase = builderBase;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sync SetExecuteAction(Action? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sync SetExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sync SetResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sync Reset()
            {
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommand Build()
            {
                return new AnonymousCommand(
                    onExecute: ExecuteAction,
                    isReadyToExecute: ExecutePredicate,
                    onReset: ResetAction,
                    name: builderBase.Name,
                    isSingle: builderBase.IsSingle,
                    isResetable: builderBase.IsResetable,
                    delayFrameCount: builderBase.DelayFrameCount
                    );
            }

            public struct Stated<TState>
            {
                private CommandBuilder builderBase;

                public TState State;
                public Action<TState>? ExecuteAction;
                public Func<TState, bool>? ExecutePredicate;
                public Action<TState>? ResetAction;

                public Stated(TState state, CommandBuilder builderBase)
                    :
                    this()
                {
                    State = state;
                    this.builderBase = builderBase;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetExecuteAction(Action<TState>? executeAction)
                {
                    ExecuteAction = executeAction;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetExecutePredicate(Func<TState, bool>? executePredicate)
                {
                    ExecutePredicate = executePredicate;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> SetResetAction(Action<TState>? resetAction)
                {
                    ResetAction = resetAction;

                    return this;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Stated<TState> Reset()
                {
                    ExecuteAction = null;
                    ExecutePredicate = null;
                    ResetAction = null;

                    return this;
                }

                [DebuggerStepThrough]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly AnonymousCommand<TState> Build()
                {
                    return new AnonymousCommand<TState>(
                        state: State,
                        onExecute: ExecuteAction,
                        isReadyToExecute: ExecutePredicate,
                        onReset: ResetAction,
                        name: builderBase.Name,
                        isSingle: builderBase.IsSingle,
                        isResetable: builderBase.IsResetable,
                        delayFrameCount: builderBase.DelayFrameCount
                        );
                }
            }
        }
    }
}
