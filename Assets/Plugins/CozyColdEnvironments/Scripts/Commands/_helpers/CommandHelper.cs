using System.Threading.Tasks;
using CCEnvs.Pools;

#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
#endif

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public static class CommandHelper
    {
#if UNITASK_PLUGIN
        public static PooledHandle<AnonymousCommandAsync<UniTask>> ToCommandPooled(
            this UniTask source
            )
        {
            return Command.Builder.SetName($"{nameof(UniTask)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<UniTask<T>>> ToCommandPooled<T>(
            this UniTask<T> source
            )
        {
            return Command.Builder.SetName($"{nameof(UniTask<T>)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }
#endif

        public static PooledHandle<AnonymousCommandAsync<ValueTask>> ToCommandPooled(
            this ValueTask source
            )
        {
            return Command.Builder.SetName($"{nameof(ValueTask)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<ValueTask<T>>> ToCommandPooled<T>(
            this ValueTask<T> source
            )
        {
            return Command.Builder.SetName($"{nameof(ValueTask<T>)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<Task>> ToCommandPooled(
            this Task source
            )
        {
            CC.Guard.IsNotNullSource(source);

            return Command.Builder.SetName($"{nameof(Task)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<Task<T>>> ToCommandPooled<T>(
            this Task<T> source
            )
        {
            CC.Guard.IsNotNullSource(source);

            return Command.Builder.SetName($"{nameof(Task<T>)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<AsyncLazy>> ToCommandPooled(
            this AsyncLazy source
            )
        {
            CC.Guard.IsNotNullSource(source);

            return Command.Builder.SetName($"{nameof(AsyncLazy)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static PooledHandle<AnonymousCommandAsync<AsyncLazy<T>>> ToCommandPooled<T>(
            this AsyncLazy<T> source
            )
        {
            CC.Guard.IsNotNullSource(source);

            return Command.Builder.SetName($"{nameof(AsyncLazy<T>)}")
                .WithState(source)
                .Asyncronously()
                .SetExecuteAction(
                static async (source, _) =>
                {
                    await source;
                })
                .BuildPooled();
        }

        public static ICommandAsync ScheduleByGlobalScheduler(this ICommandAsync source)
        {
            CC.Guard.IsNotNullSource(source);
            CC.CommandScheduler.Schedule(source);
            return source;
        }
    }
}
