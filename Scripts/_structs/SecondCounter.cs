using System;

#nullable enable
namespace UTIRLib
{
    public struct SecondCounter : IEquatable<SecondCounter>
    {
        private readonly float targetValue;
        private float value;

        public readonly float Value => value;
        public readonly float TargetValue => targetValue;
        public readonly bool TargetValueReached => targetValue > 0 && value >= targetValue;

        public SecondCounter(float startValue,
                             float targetValue = 0f)
        {
            value = startValue;
            this.targetValue = targetValue;
        }

        /// <exception cref="ArgumentException"></exception>
        public void Add(float seconds)
        {
            if (seconds <= 0)
                throw new ArgumentException(nameof(seconds));

            value += seconds;
        }

        public readonly bool Equals(SecondCounter other)
        {
            return value.NearlyEquals(other.value);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SecondCounter typed && base.Equals(typed);
        }

        public readonly override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static explicit operator float(SecondCounter secondCounter)
        {
            return secondCounter.value;
        }

        public static bool operator ==(SecondCounter left, SecondCounter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SecondCounter left, SecondCounter right)
        {
            return !left.Equals(right);
        }
    }
}
