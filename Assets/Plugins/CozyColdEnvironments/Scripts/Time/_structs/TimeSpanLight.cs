using CCEnvs.Pools;
using System;

#nullable enable
namespace CCEnvs.Dates
{
    public readonly struct TimeSpanLight : IEquatable<TimeSpanLight>, IComparable<TimeSpanLight>
    {
        public const float FROM_DAY_TO_SECOND = 86400f;
        public const float FROM_HOUR_TO_SECOND = 3600f;

        public static TimeSpanLight Empty => new();

        public readonly float Seconds { get; }

        public readonly float Minutes => Seconds / 60f;
        public readonly float Milliseconds => Seconds * 100f;
        public readonly float Hours => Minutes / 60f;

        public TimeSpanLight(float seconds)
        {
            Seconds = MathF.Max(seconds, 0f);
        }

        public static bool operator ==(TimeSpanLight left, TimeSpanLight right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimeSpanLight left, TimeSpanLight right)
        {
            return !(left == right);
        }

        public static bool operator <(TimeSpanLight left, TimeSpanLight right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(TimeSpanLight left, TimeSpanLight right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(TimeSpanLight left, TimeSpanLight right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(TimeSpanLight left, TimeSpanLight right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static TimeSpanLight operator +(TimeSpanLight left, TimeSpanLight right)
        {
            return left.Add(right);
        }

        public static TimeSpanLight operator -(TimeSpanLight left, TimeSpanLight right)
        {
            return left.Minus(right);
        }

        public static TimeSpanLight operator *(TimeSpanLight left, float right)
        {
            return left.Dot(right);
        }

        public static TimeSpanLight operator /(TimeSpanLight left, float right)
        {
            return left.Divide(right);
        }

        public static TimeSpanLight FromDays(float days)
        {
            if (days <= 0f)
                return Empty;

            return new TimeSpanLight(days );
        }

        public static TimeSpanLight FromHours(float hours)
        {
            if (hours <= 0f)
                return Empty;

            return new TimeSpanLight(hours * FROM_HOUR_TO_SECOND);
        }

        public static TimeSpanLight FromMinutes(float minutes)
        {
            if (minutes <= 0f)
                return Empty;

            return new TimeSpanLight(minutes * 60f);
        }

        public static TimeSpanLight FromMilliseconds(int milliseconds)
        {
            if (milliseconds <= 0)
                return Empty;

            return new TimeSpanLight(milliseconds * 100);
        }

        public TimeSpanLight TrimDays(out float days)
        {
            var hours = Hours;

            days = MathF.Floor(hours / 24f);

            return new TimeSpanLight(hours - days * FROM_DAY_TO_SECOND);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TimeSpanLight span && Equals(span);
        }

        public readonly bool Equals(TimeSpanLight other)
        {
            return Seconds == other.Seconds;
        }

        public TimeSpanLight Add(TimeSpanLight other)
        {
            return new TimeSpanLight(Seconds + other.Seconds);
        }

        public TimeSpanLight Minus(TimeSpanLight other)
        {
            return new TimeSpanLight(Seconds - other.Seconds);
        }

        public TimeSpanLight Dot(float multiplier)
        {
            if (multiplier <= 0f)
                return Empty;

            return new TimeSpanLight(Seconds * multiplier);
        }

        public TimeSpanLight Divide(float divider)
        {
            if (divider == 0f)
                return default;

            return new TimeSpanLight(MathF.Max(Seconds / divider, 0f));
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Seconds);
        }

        public override string ToString()
        {
            using var sb = StringBuilderPool.Shared.Get();

            sb.Value.Append(Hours);
            sb.Value.Append(':');
            sb.Value.Append(Minutes);
            sb.Value.Append(':');
            sb.Value.Append(Seconds);
            sb.Value.Append(':');
            sb.Value.Append(Milliseconds);

            return sb.Value.ToString();
        }

        public readonly int CompareTo(TimeSpanLight other)
        {
            return Seconds.CompareTo(other.Seconds);
        }
    }
}
