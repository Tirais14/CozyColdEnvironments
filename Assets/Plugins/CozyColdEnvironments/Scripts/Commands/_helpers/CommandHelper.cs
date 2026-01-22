using CommunityToolkit.Diagnostics;

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
        public static ICommandAsync TaskToCommand(this UniTask source)
        {
            return new FromUniTaskCommand(source);
        }

        public static ICommandAsync TaskToCommand<T>(this UniTask<T> source)
        {
            return new FromUniTaskCommand(source);
        }
#endif

        public static ICommandAsync TaskToCommand(this ValueTask source)
        {
            return new FromValueTaskCommand(source);
        }

        public static ICommandAsync TaskToCommand<T>(this ValueTask<T> source)
        {
            return new FromValueTaskCommand<T>(source);
        }

        public static ICommandAsync TaskToCommand(this Task source)
        {
            CC.Guard.IsNotNullSource(source);
            return new FromTaskCommand(source);
        }

        public static ICommandAsync ScheduleByGlobalScheduler(this ICommandAsync source)
        {
            CC.Guard.IsNotNullSource(source);
            CC.CommandScheduler.Schedule(source);
            return source;
        }
    }
}
