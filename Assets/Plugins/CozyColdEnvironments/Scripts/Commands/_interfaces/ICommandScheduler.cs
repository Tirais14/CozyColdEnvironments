#nullable enable
using CCEnvs.Returnables;
using R3;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler
    {
        bool HasCommands { get; }

        void AddCommand(ICommand command);

        void Clear();

        void DoTick();

        Observable<ICommand> ObserveAddCommand();

        Observable<Mock> ObserveCommandsExecuted();
    }
}
