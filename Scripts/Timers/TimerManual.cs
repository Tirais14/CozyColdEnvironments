using System;

#nullable enable
#pragma warning disable S2328
namespace UTIRLib.Timers
{
    public sealed class TimerManual : ITimer, IEquatable<TimerManual>
    {
        private float seconds;
        private bool targetReachedCallbackInvoked;

        public event Action? OnTargetReached;

        public float Seconds => seconds;
        public float TargetValue { get; set; }
        public bool TargetValueReached => TargetValue > 0 && seconds >= TargetValue;
        bool ITimer.IsExecuting => true;

        public TimerManual(float seconds)
        {
            this.seconds = seconds;
        }

        public TimerManual() : this(seconds: 0)
        {
        }

        /// <exception cref="ArgumentException"></exception>
        public void AddSeconds(float seconds)
        {
            if (seconds < 0)
                throw new ArgumentException(nameof(seconds));

            this.seconds += seconds;

            if (TargetValueReached && !targetReachedCallbackInvoked)
            {
                OnTargetReached?.Invoke();
                targetReachedCallbackInvoked = true;
            }
        }

        public ITimer ResetTimer()
        {
            seconds = 0f;
            targetReachedCallbackInvoked = false;

            return this;
        }

        public bool Equals(TimerManual? other)
        {
            if (other is null)
                return false;

            return seconds.NearlyEquals(other.seconds)
                   &&
                   TargetValue.NearlyEquals(other.TargetValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimerManual typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(seconds, TargetValue);
        }

        public static explicit operator float(TimerManual? timer)
        {
            if (timer is null)
                return -1f;

            return timer.seconds;
        }

        public static implicit operator bool(TimerManual timer)
        {
            return timer is not null && timer.TargetValueReached;
        }

        public static bool operator ==(TimerManual? left, TimerManual? right)
        {
            if (left is null && right is null)
                return true;
            if (left is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(TimerManual? left, TimerManual? right)
        {
            if (left is null && right is null)
                return false;
            if (left is null)
                return true;

            return !left.Equals(right);
        }

        public static TimerManual operator +(TimerManual left, float right)
        {
            if (left is null)
                return new TimerManual(right);

            left.AddSeconds(right);
            return left;
        }

        ITimer ITimer.StartTimer() => this;

        ITimer ITimer.StopTimer() => this;
    }
}
