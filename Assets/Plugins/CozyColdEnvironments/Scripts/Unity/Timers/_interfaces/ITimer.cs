using R3;
using System;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public interface ITimer
    {
        /// <summary>Input is <see cref="Interval"/></summary>
        Observable<TimeSpan> OnTick { get; }
        /// <summary>Input is <see cref="Elapsed"/></summary>
        Observable<TimeSpan> OnTargetReached { get; }

        TimeSpan Elapsed { get; }
        TimeSpan? Target { get; set; }
        TimeSpan Interval { get; }
        bool TargetReached { get; }
        bool IsEnabled { get; }
        TimerOptions Options { get; set; }

        ITimer StartTimer();

        ITimer StopTimer();

        ITimer ResetTimer();
    }
}
