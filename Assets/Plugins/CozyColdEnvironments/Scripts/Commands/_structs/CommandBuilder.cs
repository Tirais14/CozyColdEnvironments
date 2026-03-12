using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CCEnvs.Attributes;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;

#nullable enable
#pragma warning disable S3218
namespace CCEnvs.Patterns.Commands
{
    public struct CommandBuilder
    {
        public string? Name;

        public bool IsSingle;
        public bool ExecuteOnThreadPool;

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CommandBuilder Create()
        {
            return new CommandBuilder().Reset();
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder WithName(string? name = null)
        {
            Name = name;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandBuilder WithName<T>(string? name, T invocationSource)
        {
            Name = $"{invocationSource.GetTypeName(TypeNameConvertingAttributes.IncludeGenericArguments)}.{name ?? "???"}";

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
        public CommandBuilder OnThreadPool(bool state = true)
        {
            ExecuteOnThreadPool = state;

            return this;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Intermediate WithoutState()
        {
            return new Intermediate(this);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Intermediate<TState> WithState<TState>(TState state)
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
            public Intermediate WithExecutePredicate(Func<bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate WithResetAction(Action? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate WithCancelAction(Action? cancelAction)
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
            public Intermediate<TState> WithExecutePredicate(Func<TState, bool>? executePredicate)
            {
                ExecutePredicate = executePredicate;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate<TState> WithResetAction(Action<TState>? resetAction)
            {
                ResetAction = resetAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Intermediate<TState> WithCancelAction(Action<TState>? cancelAction)
            {
                CancelAction = cancelAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Sync<TState> Synchronously()
            {
                Guard.IsNotNull(State, nameof(State));

                return new Sync<TState>(builder, this);
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Async<TState> Asynchronously()
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
            public Async WithExecuteAction(Func<CancellationToken, ValueTask>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommandAsync Build()
            {
                return new AnonymousCommandAsync()
                {
                    Name = builder.Name ?? string.Empty,
                    IsSingle = builder.IsSingle,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction,
                    ExecuteOnThreadPool = builder.ExecuteOnThreadPool
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledObject<AnonymousCommandAsync> BuildPooled()
            {
                PooledObject<AnonymousCommandAsync> pooledCmd;
                AnonymousCommandAsync cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommandAsync();

                    pool ??= new ObjectPool<AnonymousCommandAsync>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.Name = builder.Name ?? string.Empty;
                cmd.IsSingle = builder.IsSingle;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;
                cmd.ExecuteOnThreadPool = builder.ExecuteOnThreadPool;

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
            public Async<TState> WithExecuteAction(Func<TState, CancellationToken, ValueTask>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommandAsync<TState> Build()
            {
                return new AnonymousCommandAsync<TState>()
                {
                    Name = builder.Name ?? string.Empty,
                    IsSingle = builder.IsSingle,
                    State = intermediate.State,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction,
                    ExecuteOnThreadPool = builder.ExecuteOnThreadPool
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledObject<AnonymousCommandAsync<TState>> BuildPooled()
            {
                PooledObject<AnonymousCommandAsync<TState>> pooledCmd = default;
                AnonymousCommandAsync<TState> cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommandAsync<TState>();

                    pool ??= new ObjectPool<AnonymousCommandAsync<TState>>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.Name = builder.Name ?? string.Empty;
                cmd.IsSingle = builder.IsSingle;
                cmd.State = intermediate.State;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;
                cmd.ExecuteOnThreadPool = builder.ExecuteOnThreadPool;

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
            public Sync WithExecuteAction(Action? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommand Build()
            {
                return new AnonymousCommand()
                {
                    Name = builder.Name ?? string.Empty,
                    IsSingle = builder.IsSingle,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction,
                    ExecuteOnThreadPool = builder.ExecuteOnThreadPool
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledObject<AnonymousCommand> BuildPooled()
            {
                PooledObject<AnonymousCommand> pooledCmd;
                AnonymousCommand cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommand(
                        );

                    pool ??= new ObjectPool<AnonymousCommand>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.Name = builder.Name ?? string.Empty;
                cmd.IsSingle = builder.IsSingle;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;
                cmd.ExecuteOnThreadPool = builder.ExecuteOnThreadPool;

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
            public Sync<TState> WithExecuteAction(Action<TState>? executeAction)
            {
                ExecuteAction = executeAction;

                return this;
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly AnonymousCommand<TState> Build()
            {
                return new AnonymousCommand<TState>()
                {
                    Name = builder.Name ?? string.Empty,
                    IsSingle = builder.IsSingle,
                    State = intermediate.State,
                    ExecuteAction = ExecuteAction,
                    ExecutePredicate = intermediate.ExecutePredicate,
                    ResetAction = intermediate.ResetAction,
                    CancelAction = intermediate.CancelAction,
                    ExecuteOnThreadPool = builder.ExecuteOnThreadPool
                };
            }

            [DebuggerStepThrough]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PooledObject<AnonymousCommand<TState>> BuildPooled()
            {
                PooledObject<AnonymousCommand<TState>> pooledCmd;
                AnonymousCommand<TState> cmd;

                if (pool is not null && pool.InactiveCount > 0)
                {
                    pooledCmd = pool.Get();
                    cmd = pooledCmd.Value;
                }
                else
                {
                    cmd = new AnonymousCommand<TState>();

                    pool ??= new ObjectPool<AnonymousCommand<TState>>();

                    pool.Return(cmd);
                    pooledCmd = pool.Get();
                }

                cmd.Name = builder.Name ?? string.Empty;
                cmd.IsSingle = builder.IsSingle;
                cmd.State = intermediate.State;
                cmd.ExecuteAction = ExecuteAction;
                cmd.ExecutePredicate = intermediate.ExecutePredicate;
                cmd.ResetAction = intermediate.ResetAction;
                cmd.CancelAction = intermediate.CancelAction;
                cmd.ExecuteOnThreadPool = builder.ExecuteOnThreadPool;

                return pooledCmd;
            }
        }
    }
}
