using System;

#nullable enable
#pragma warning disable S2328
namespace UTIRLib
{
    public sealed class TimerManual : IEquatable<TimerManual>
    {
        private readonly float targetValue;
        private float value;

        public event Action<TimerManual>? OnTargetReached;

        public float Value => value;
        public float TargetValue => targetValue;
        public bool TargetValueReached => targetValue > 0 && value >= targetValue;

        public TimerManual(float startValue,
                           float targetValue = 0f)
        {
            value = startValue;
            this.targetValue = targetValue;
        }

        /// <exception cref="ArgumentException"></exception>
        public void AddSeconds(float seconds)
        {
            if (seconds <= 0)
                throw new ArgumentException(nameof(seconds));

            value += seconds;

            if (TargetValueReached)
                OnTargetReached?.Invoke(this);
        }

        public bool Equals(TimerManual? other)
        {
            if (other is null)
                return false;

            return value.NearlyEquals(other.value)
                   && 
                   targetValue.NearlyEquals(other.targetValue);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimerManual typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, targetValue);
        }

        public static explicit operator float(TimerManual? secondCounter)
        {
            if (secondCounter is null)
                return -1f;

            return secondCounter.value;
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
    }
}
