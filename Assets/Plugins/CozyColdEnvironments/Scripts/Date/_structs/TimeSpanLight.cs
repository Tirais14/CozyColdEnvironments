using CCEnvs.Pools;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public readonly struct TimeSpanLight : IEquatable<TimeSpanLight>, IComparable<TimeSpanLight>
    {
        public const float FROM_DAY_TO_SECOND = 86400f;
        public const float FROM_HOUR_TO_SECOND = 3600f;
        public const float FROM_HOUR_TO_MILLISECOND = 360000f;

        public static TimeSpanLight Empty => new();

        [DataMember(Name = "seconds")]
        public readonly float Seconds { get; }

        public readonly float Minutes => Seconds / 60f;
        public readonly float Milliseconds => Seconds * 100f;
        public readonly float Hours => Minutes / 60f;

#if JSON_NET
        [JsonConstructor]
#endif
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

        public static implicit operator TimeSpanLight(float seconds)
        {
            return new TimeSpanLight(seconds);
        }

        public static implicit operator float(TimeSpanLight instance)
        {
            return instance.Seconds;
        }

        public static TimeSpanLight operator +(TimeSpanLight left, TimeSpanLight right)
        {
            return left.Add(right);
        }
        public static TimeSpanLight operator +(TimeSpanLight left, float right)
        {
            return left.Add(right);
        }

        public static TimeSpanLight operator -(TimeSpanLight left, TimeSpanLight right)
        {
            return left.Minus(right);
        }
        public static TimeSpanLight operator -(TimeSpanLight left, float right)
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

            return new TimeSpanLight(days);
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

        public readonly TimeSpanLight TrimDays(out float days)
        {
            var hours = Hours;

            days = MathF.Floor(hours / 24f);

            return new TimeSpanLight(hours - days * FROM_DAY_TO_SECOND);
        }

        public readonly TimeSpanLight Add(TimeSpanLight other)
        {
            return new TimeSpanLight(Seconds + other.Seconds);
        }
        public readonly TimeSpanLight Add(float seconds)
        {
            return new TimeSpanLight(Seconds + seconds);
        }

        public readonly TimeSpanLight Minus(TimeSpanLight other)
        {
            return new TimeSpanLight(Seconds - other.Seconds);
        }
        public readonly TimeSpanLight Minus(float seconds)
        {
            return new TimeSpanLight(Seconds - seconds);
        }

        public readonly TimeSpanLight Dot(float multiplier)
        {
            if (multiplier <= 0f)
                return Empty;

            return new TimeSpanLight(Seconds * multiplier);
        }

        public readonly TimeSpanLight Divide(float divider)
        {
            if (divider == 0f)
                return default;

            return new TimeSpanLight(MathF.Max(Seconds / divider, 0f));
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TimeSpanLight span && Equals(span);
        }

        public readonly bool Equals(TimeSpanLight other)
        {
            return Seconds == other.Seconds;
        }

        public readonly bool NearlyEquals(TimeSpanLight other, float? epsilon = null)
        {
            if (Equals(other))
                return true;

            return Seconds.NearlyEquals(other.Seconds, epsilon);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Seconds);
        }

        public readonly string ToString(StringFormat format)
        {
            if (format == StringFormat.None)
                return "None";

            using var sb = StringBuilderPool.Shared.Get();

            bool writen = false;

            float hours = MathF.Floor(Hours);

            if (format.HasFlagT(StringFormat.IncludeHours))
            {
                sb.Value.Append((int)hours);
                writen = true;
            }

            if (format.HasFlagT(StringFormat.IncludeMinutes))
            {
                if (writen)
                    sb.Value.Append(':');

                float minutes = Minutes - (hours * 60f);

                sb.Value.Append((int)minutes);
                writen = true;
            }

            if (format.HasFlagT(StringFormat.IncludeSeconds))
            {
                if (writen)
                    sb.Value.Append(':');

                float seconds = Seconds - (MathF.Floor(Minutes) * 60f);

                sb.Value.Append((int)seconds);
                writen = true;
            }

            if (format.HasFlagT(StringFormat.IncludeMilliseconds))
            {
                if (writen)
                    sb.Value.Append(':');

                float milliseconds = Milliseconds - (MathF.Floor(Seconds) * 100f);

                sb.Value.Append((int)milliseconds);
                writen = true;
            }

            return sb.Value.ToString();
        }

        public readonly override string ToString()
        {
            return ToString(StringFormat.Default);
        }

        public readonly int CompareTo(TimeSpanLight other)
        {
            return Seconds.CompareTo(other.Seconds);
        }

        [Flags]
        public enum StringFormat
        {
            None,
            IncludeHours,
            IncludeMinutes,
            IncludeSeconds,
            IncludeMilliseconds,
            Default = IncludeHours | IncludeMinutes
        }
    }
}
