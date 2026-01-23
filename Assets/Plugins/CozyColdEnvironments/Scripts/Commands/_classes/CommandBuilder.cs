using System;
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

        public static CommandBuilder Create()
        {
            return new CommandBuilder().Reset();
        }

        public CommandBuilder SetName(string? name = null)
        {
            this.Name = name;

            return this;
        }

        public CommandBuilder SetIsSingleCommand(bool state = true)
        {
            IsSingle = state;

            return this;
        }

        public CommandBuilder SetIsResetable(bool state = true)
        {
            IsResetable = state; 

            return this; 
        }

        public CommandBuilder SetDelayFrames(int count)
        {
            DelayFrameCount = count;

            return this;
        }

        public readonly Sync AsSync() => new(this);

        public readonly Sync.Stated<TState> AsSync<TState>(TState state)
        {
            return new Sync.Stated<TState>(state, this);
        }

        public readonly Async AsAsync() => new(this);

        public readonly Async.Stated<TState> AsAsync<TState>(TState state)
        {
            return new Async.Stated<TState>(state, this);
        }

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

            public Async SetExecuteAction(Func<CancellationToken, ValueTask>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            public Async SetExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }


            public Async SetResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            public Async Reset()
            {
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;

                return this;
            }

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

                public Stated<TState> SetExecuteAction(Func<TState, CancellationToken, ValueTask>? executeAction)
                {
                    ExecuteAction = executeAction;

                    return this;
                }

                public Stated<TState> SetExecutePredicate(Func<TState, bool>? executePredicate)
                {
                    ExecutePredicate = executePredicate;

                    return this;
                }


                public Stated<TState> SetResetAction(Action<TState>? resetAction)
                {
                    ResetAction = resetAction;

                    return this;
                }

                public Stated<TState> Reset()
                {
                    ExecuteAction = null;
                    ExecutePredicate = null;
                    ResetAction = null;

                    return this;
                }

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

            public Sync SetExecuteAction(Action? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            public Sync SetExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }


            public Sync SetResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            public Sync Reset()
            {
                ExecuteAction = null;
                ExecutePredicate = null;
                ResetAction = null;

                return this;
            }

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

                public Stated<TState> SetExecuteAction(Action<TState>? executeAction)
                {
                    ExecuteAction = executeAction;

                    return this;
                }

                public Stated<TState> SetExecutePredicate(Func<TState, bool>? executePredicate)
                {
                    ExecutePredicate = executePredicate;

                    return this;
                }


                public Stated<TState> SetResetAction(Action<TState>? resetAction)
                {
                    ResetAction = resetAction;

                    return this;
                }

                public Stated<TState> Reset()
                {
                    ExecuteAction = null;
                    ExecutePredicate = null;
                    ResetAction = null;

                    return this;
                }

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
