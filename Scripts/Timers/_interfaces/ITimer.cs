using System;

#nullable enable
namespace UTIRLib.Timers
{
    public interface ITimer : IStateSwitchable
    {
        event Action OnTargetReached;

        float Seconds { get; }
        float TargetValue { get; }
        bool TargetValueReached { get; }
        bool IsExecuting { get; }

        bool IStateSwitchable.IsEnabled => IsExecuting;

        ITimer StartTimer();

        ITimer StopTimer();

        ITimer ResetTimer();

        void IStateSwitchable.Enable() => StartTimer();

        void IStateSwitchable.Disable() => StopTimer();
    }
}
