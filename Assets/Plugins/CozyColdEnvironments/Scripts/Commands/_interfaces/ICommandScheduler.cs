#nullable enable
using R3;
using System;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler : ISwitchable
    {
        bool HasCommands { get; }
        bool IsRunning { get; }

        void Schedule(ICommand command);

        void Reset();

        void DoFrame();

        Observable<ICommand> ObserveAddCommand();

        Observable<bool> ObserveIsRunningFinsihed();

        Observable<bool> ObserveIsRunningStarted();
    }
}
