using System;

#nullable enable
namespace UTIRLib.Timers
{
    public interface ITimer
    {
        event Action OnTargetReached;

        float Seconds { get; }
        float TargetValue { get; set; }
        bool TargetValueReached { get; }
        bool IsActive { get; }
        bool IsOnTargetReachedInvoked { get; }
        TimerOptions Options { get; set; }

        ITimer StartTimer();

        ITimer StopTimer();

        ITimer ResetTimer();

        TimeSpan GetTimeSpan();
    }
}
