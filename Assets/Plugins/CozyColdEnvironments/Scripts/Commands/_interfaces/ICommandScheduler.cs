#nullable enable
using R3;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler
    {
        bool HasCommands { get; }

        void Schedule(ICommand command);

        void Reset();

        void DoTick();

        Observable<ICommand> ObserveAddCommand();

        Observable<Unit> ObserveCommandsExecuted();
    }
}
