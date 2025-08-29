using System;

#nullable enable
#pragma warning disable S2328
namespace CozyColdEnvironments.Timers
{
    public sealed class TimerManual : ITimer, IEquatable<TimerManual>
    {
        public event Action? OnTargetReached;

        public float TargetValue { get; set; }
        public TimerOptions Options { get; set; }
        public float Seconds { get; private set; }
        public bool TargetValueReached => TargetValue > 0 && Seconds >= TargetValue;
        public bool IsOnTargetReachedInvoked { get; private set; }
        public bool IsActive { get; private set; }

        public TimerManual()
        {
        }

        public TimeSpan GetTimeSpan()
        {
            return TimeSpan.FromSeconds(Seconds);
        }

        public ITimer StartTimer()
        {
            IsActive = true;

            return this;
        }

        public ITimer StopTimer()
        {
            IsActive = false;

            return this;
        }

        /// <exception cref="ArgumentException"></exception>
        public void AddSeconds(float seconds)
        {
            if (seconds < 0)
                throw new ArgumentException(nameof(seconds));
            if (!IsActive)
                return;

            Seconds += seconds;

            if (!TargetValueReached && IsOnTargetReachedInvoked)
                IsOnTargetReachedInvoked = false;

            if (TargetValueReached
                &&
                !IsOnTargetReachedInvoked)
            {
                if (OnTargetReached is not null)
                {
                    OnTargetReached();
                    IsOnTargetReachedInvoked = true;
                }

                if (Options.IsFlagSetted(TimerOptions.StopOnTargetReached))
                    StopTimer();

                if (Options.IsFlagSetted(TimerOptions.ResetOnTargetReached))
                    ResetTimer();
            }
        }

        public ITimer ResetTimer()
        {
            Seconds = 0f;
            IsOnTargetReachedInvoked = false;

            return this;
        }

        public bool Equals(TimerManual? other)
        {
            if (other is null)
                return false;

            return Seconds.NearlyEquals(other.Seconds)
                   &&
                   TargetValue.NearlyEquals(other.TargetValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimerManual typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Seconds, TargetValue);
        }

        public static explicit operator float(TimerManual? timer)
        {
            if (timer is null)
                return -1f;

            return timer.Seconds;
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
    }
}
