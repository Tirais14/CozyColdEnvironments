#nullable enable
using R3;
using System;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler : ISwitchable
    {
        bool HasCommands { get; }
        bool IsRunning { get; }

        int DelayFrameCountBeforeRunningFinished { get; set; }

        void Schedule(ICommand command);

        void Reset();

        void OnFrame();

        Observable<ICommand> ObserveAddCommand();

        Observable<bool> ObserveIsRunningFinsihed();

        Observable<bool> ObserveIsRunningStarted();
    }
}
