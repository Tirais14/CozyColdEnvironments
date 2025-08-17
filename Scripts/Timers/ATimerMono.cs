#nullable enable
using System;

namespace UTIRLib.Timers
{
    public class ATimerMono : MonoX, ITimer
    {
        protected readonly TimerManual timer = new();

        public event Action OnTargetReached {
            add => timer.OnTargetReached += value;
            remove => timer.OnTargetReached -= value;
        }

        public ITimer Timer => timer;
        public float Seconds => timer.Seconds;
        public float TargetValue => timer.TargetValue;
        public bool TargetValueReached => timer.TargetValueReached;
        public bool IsExecuting { get; protected set; }

        public ITimer StartTimer()
        {
            IsExecuting = true;
            enabled = IsExecuting;

            return this;
        }

        public ITimer StopTimer()
        {
            IsExecuting = false;
            enabled = IsExecuting;

            return this;
        }

        public ITimer ResetTimer()
        {
            return timer.ResetTimer();
        }
    }
}
