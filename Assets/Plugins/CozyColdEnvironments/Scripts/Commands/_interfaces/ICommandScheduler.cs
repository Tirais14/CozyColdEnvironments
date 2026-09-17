#nullable enable
using R3;
using System.Collections.Generic;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandScheduler : ISwitchable
    {
        string Name { get; }

        bool HasCommands { get; }
        bool IsRunning { get; }

        int DelayFrameCountBeforeRunningFinished { get; set; }
        int CommandCount { get; }

        IEnumerable<ICommandBase> Commands { get; }

        void Schedule(ICommandBase command);

        void Reset();

        void OnFrame();

        bool HasCommand(ICommandBase? command);

        bool HasCommand(CommandSignature commandSignature);

        Observable<ICommandBase> ObserveScheduleCommand();

        Observable<bool> ObserveRunningFinsihed();

        Observable<bool> ObserveRunningStarted();
    }
}
