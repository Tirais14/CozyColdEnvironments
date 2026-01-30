using System.Threading.Tasks;

#if UNITASK_PLUGIN
using Cysharp.Threading.Tasks;
#endif

#nullable enable
namespace CCEnvs.Patterns.Commands
{
    public static class CommandHelper
    {
#if UNITASK_PLUGIN
        public static ICommandAsync ToCommand(this UniTask source)
        {
            return new FromUniTaskCommand()
            {
                Task = source
            };
        }

        public static ICommandAsync ToCommand<T>(this UniTask<T> source)
        {

            return new FromUniTaskCommand()
            {
                Task = source
            };
        }
#endif

        public static ICommandAsync ToCommand(this ValueTask source)
        {
            return new FromValueTaskCommand()
            {
                Task = source
            };
        }

        public static ICommandAsync ToCommand<T>(this ValueTask<T> source)
        {
            return new FromValueTaskCommand<T>()
            {
                Task = source
            };
        }

        public static ICommandAsync ToCommand(this Task source)
        {
            CC.Guard.IsNotNullSource(source);

            return new FromTaskCommand()
            {
                Task = source
            };
        }

        public static ICommandAsync ToCommand<T>(this Task<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            return new FromTaskCommand<T>()
            {
                Task = source
            };
        }

        public static ICommandAsync ToCommand(this AsyncLazy source)
        {
            CC.Guard.IsNotNullSource(source);

            return new FromAsyncLazyCommand()
            {
                TaskLazy = source
            };
        }

        public static ICommandAsync ToCommand<T>(this AsyncLazy<T> source)
        {
            CC.Guard.IsNotNullSource(source);

            return new FromAsyncLazyCommand<T>()
            {
                TaskLazy = source
            };
        }

        public static ICommandAsync ScheduleByGlobalScheduler(this ICommandAsync source)
        {
            CC.Guard.IsNotNullSource(source);
            CC.CommandScheduler.Schedule(source);
            return source;
        }
    }
}
