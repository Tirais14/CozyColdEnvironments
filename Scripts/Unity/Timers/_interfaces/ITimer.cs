using CCEnvs.Returnables;
using CCEnvs.Rx;
using System;

#nullable enable
namespace CCEnvs.Unity.Timers
{
    public interface ITimer
    {
        /// <summary>Input is <see cref="Interval"/></summary>
        IObservable<TimeSpan> OnTick { get; }
        /// <summary>Input is <see cref="Elapsed"/></summary>
        IObservable<TimeSpan> OnTargetReached { get; }

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
