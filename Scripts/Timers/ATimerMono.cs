#nullable enable
using System;

namespace UTIRLib.Timers
{
    public class ATimerMono : MonoX, IStateToggleable, ITimer
    {
        private bool isEnabled;
        protected readonly TimerManual timer = new();

        public ITimer Timer => timer;
        public bool IsEnabled {
            get => isEnabled && enabled;
            set
            {
                isEnabled = value;
                enabled = value;
            }
        }
        public float Seconds => timer.Seconds;
        public float TargetValue => timer.TargetValue;
        public bool TargetValueReached => timer.TargetValueReached;
        public event Action<ITimer> OnTargetReached {
            add => ((ITimer)timer).OnTargetReached += value;
            remove => ((ITimer)timer).OnTargetReached -= value;
        }
    }
}
