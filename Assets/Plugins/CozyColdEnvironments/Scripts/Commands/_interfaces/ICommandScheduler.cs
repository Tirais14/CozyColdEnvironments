#nullable enable
using R3;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler : ISwitchable
    {
        string Name { get; }

        bool HasCommands { get; }
        bool IsRunning { get; }

        int DelayFrameCountBeforeRunningFinished { get; set; }

        void Schedule(ICommandBase command);

        void Reset();

        void OnFrame();

        bool HasCommand(ICommandBase? command);

        bool HasCommand(CommandSignature commandSignature);

        Observable<ICommandBase> ObserveAddCommand();

        Observable<bool> ObserveIsRunningFinsihed();

        Observable<bool> ObserveIsRunningStarted();
    }
}
