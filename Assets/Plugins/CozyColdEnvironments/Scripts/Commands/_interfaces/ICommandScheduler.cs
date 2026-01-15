#nullable enable
using R3;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler
    {
        bool HasCommands { get; }
        bool IsEnabled { get; }

        void Schedule(ICommand command);

        void Reset();

        void DoTick();

        void Enable();

        void Disable();

        Observable<ICommand> ObserveAddCommand();

        Observable<Unit> ObserveCommandsExecuted();
    }
}
