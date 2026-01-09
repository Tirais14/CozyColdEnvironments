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
        public static ICommand TaskToCommand(this UniTask source)
        {
            return new FromUniTaskCommand(source);
        }

        public static ICommand TaskToCommand<T>(this UniTask<T> source)
        {
            return new FromUniTaskCommand(source);
        }
#endif

        public static ICommand TaskToCommand(this ValueTask source)
        {
            return new FromValueTaskCommand(source);
        }

        public static ICommand TaskToCommand<T>(this ValueTask<T> source)
        {
            return new FromValueTaskCommand<T>(source);
        }

        public static ICommand TaskToCommand(this Task source)
        {
            CC.Guard.IsNotNullSource(source);
            return new FromTaskCommand(source);
        }

        public static ICommand ScheduleByGlobalScheduler(this ICommand source)
        {
            CC.Guard.IsNotNullSource(source);
            CC.CommandScheduler.Schedule(source);
            return source;
        }
    }
}
