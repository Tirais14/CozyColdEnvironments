using CCEnvs.Attributes;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
#pragma warning disable S3218
namespace CCEnvs.Patterns.Commands
{
    public struct CommandBuilder
    {
        public string? Name;

        public bool IsSingle;

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
            Name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder SetSingle(bool state = true)
        {
            IsSingle = state;

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
        public readonly Intermediate NextStep()
        {
            return new Intermediate(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Intermediate<TState> NextStep<TState>(TState state)
        {
            Guard.IsNotNull(state, nameof(state));

            return new Intermediate<TState>(this)
            {
                State = state
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder Reset()
        {
            Name = null;
            IsSingle = false;
            DelayFrameCount = 0;

            return this;
        }

        public struct Intermediate 
        {
            private readonly CommandBuilder builder;

            public Func<bool>? ExecutePredicate;
            public Action? ResetAction;
            public Action? CancelAction;

            public Intermediate(CommandBuilder builder)
                :
                this()
            {
                this.builder = builder;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate SetExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate SetResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate SetCancelAction(Action? cancelAction)
            {
                CancelAction = cancelAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Sync Syncronously() => new(builder, this);

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Async Asyncronously() => new(builder, this);
        }

        public struct Intermediate<TState>
        {
            private readonly CommandBuilder builder;

            public TState State;
            public Func<TState, bool>? ExecutePredicate;
            public Action<TState>? ResetAction;
            public Action<TState>? CancelAction;

            public Intermediate(CommandBuilder builder)
                :
                this()
            {
                this.builder = builder;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate<TState> SetExecutePredicate(Func<TState, bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate<TState> SetResetAction(Action<TState>? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate<TState> SetCancelAction(Action<TState>? cancelAction)
            {
                CancelAction = cancelAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Sync<TState> Syncronously()
            {
                Guard.IsNotNull(State, nameof(State));

                return new Sync<TState>(builder, this);
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Async<TState> Asyncronously()
            {
                Guard.IsNotNull(State, nameof(State));

                return new Async<TState>(builder, this);
            }
        }

        public struct Async
        {
            [OnInstallResetable]
            private static ObjectPool<AnonymousCommandAsync>? pool;

            private readonly CommandBuilder builder;
            private readonly Intermediate intermediate;

            public Func<CancellationToken, ValueTask>? ExecuteAction;

            public Async(CommandBuilder builder, Intermediate intermediate)
                :
                this()
            {
                this.builder = builder;
                this.intermediate = intermediate;
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
            public readonly AnonymousCommandAsync Build()
            {
                return new AnonymousCommandAsync(
                    name: builder.Name,
                    isSingle: builder.IsSingle,
                    delayFrameCount: builder.DelayFrameCount)
                {
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledHandle<AnonymousCommandAsync> BuildPooled()
            {
                PooledHandle<AnonymousCommandAsync> pooledCmd;
                AnonymousCommandAsync cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommandAsync(
                        name: builder.Name,
                        isSingle: builder.IsSingle,
                        delayFrameCount: builder.DelayFrameCount
                        );

                    pool ??= new ObjectPool<AnonymousCommandAsync>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;

                return pooledCmd;
            }
        }

        public struct Async<TState>
        {
            [OnInstallResetable]
            private static ObjectPool<AnonymousCommandAsync<TState>>? pool;

            private readonly CommandBuilder builder;
            private readonly Intermediate<TState> intermediate;

            public Func<TState, CancellationToken, ValueTask>? ExecuteAction;

            public Async(CommandBuilder builder, Intermediate<TState> intermediate)
                :
                this()
            {
                this.builder = builder;
                this.intermediate = intermediate;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Async<TState> SetExecuteAction(Func<TState, CancellationToken, ValueTask>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommandAsync<TState> Build()
            {
                return new AnonymousCommandAsync<TState>(
                    name: builder.Name,
                    isSingle: builder.IsSingle,
                    delayFrameCount: builder.DelayFrameCount)
                {
                    State = intermediate.State,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledHandle<AnonymousCommandAsync<TState>> BuildPooled()
            {
                PooledHandle<AnonymousCommandAsync<TState>> pooledCmd;
                AnonymousCommandAsync<TState> cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommandAsync<TState>(
                        name: builder.Name,
                        isSingle: builder.IsSingle,
                        delayFrameCount: builder.DelayFrameCount
                        );

                    pool ??= new ObjectPool<AnonymousCommandAsync<TState>>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.State = intermediate.State;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;

                return pooledCmd;
            }
        }

        public struct Sync
        {
            [OnInstallResetable]
            private static ObjectPool<AnonymousCommand>? pool;

            private readonly CommandBuilder builder;
            private readonly Intermediate intermediate;

            public Action? ExecuteAction;

            public Sync(CommandBuilder builder, Intermediate intermediate)
                :
                this()
            {
                this.builder = builder;
                this.intermediate = intermediate;
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
            public readonly AnonymousCommand Build()
            {
                return new AnonymousCommand(
                    name: builder.Name,
                    isSingle: builder.IsSingle,
                    delayFrameCount: builder.DelayFrameCount)
                {
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledHandle<AnonymousCommand> BuildPooled()
            {
                PooledHandle<AnonymousCommand> pooledCmd;
                AnonymousCommand cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommand(
                        name: builder.Name,
                        isSingle: builder.IsSingle,
                        delayFrameCount: builder.DelayFrameCount
                        );

                    pool ??= new ObjectPool<AnonymousCommand>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;

                return pooledCmd;
            }
        }

        public struct Sync<TState>
        {
            [OnInstallResetable]
            private static ObjectPool<AnonymousCommand<TState>>? pool;

            private readonly CommandBuilder builder;
            private readonly Intermediate<TState> intermediate;

            public Action<TState>? ExecuteAction;

            public Sync(CommandBuilder builder, Intermediate<TState> intermediate)
                :
                this()
            {
                this.builder = builder;
                this.intermediate = intermediate;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sync<TState> SetExecuteAction(Action<TState>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommand<TState> Build()
            {
                return new AnonymousCommand<TState>(
                    name: builder.Name,
                    isSingle: builder.IsSingle,
                    delayFrameCount: builder.DelayFrameCount)
                {
                    State = intermediate.State,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledHandle<AnonymousCommand<TState>> BuildPooled()
            {
                PooledHandle<AnonymousCommand<TState>> pooledCmd;
                AnonymousCommand<TState> cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommand<TState>(
                        name: builder.Name,
                        isSingle: builder.IsSingle,
                        delayFrameCount: builder.DelayFrameCount
                        );

                    pool ??= new ObjectPool<AnonymousCommand<TState>>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.State = intermediate.State;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;

                return pooledCmd;
            }
        }
    }
}
