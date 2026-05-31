using CCEnvs.Pools;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public readonly struct TimeSpanFloat : IEquatable<TimeSpanFloat>, IComparable<TimeSpanFloat>
    {
        public const float FROM_DAY_TO_SECOND = 86400f;
        public const float FROM_HOUR_TO_SECOND = 3600f;
        public const float FROM_HOUR_TO_MILLISECOND = 360000f;

        public static TimeSpanFloat Empty => new();

        [DataMember(Name = "seconds")]
        public readonly float Seconds { get; }

        public readonly float Minutes => Seconds / 60f;
        public readonly float Milliseconds => Seconds * 100f;
        public readonly float Hours => Minutes / 60f;

#if JSON_NET
        [JsonConstructor]
#endif
        public TimeSpanFloat(float seconds)
        {
            Seconds = seconds;
        }

        #region Cast Operators

        public static implicit operator TimeSpanFloat(float seconds)
        {
            return new TimeSpanFloat(seconds);
        }

        public static implicit operator float(TimeSpanFloat instance)
        {
            return instance.Seconds;
        }

        public static explicit operator TimeSpanFloat(TimeSpan timeSpan)
        {
            return new TimeSpanFloat((float)timeSpan.TotalSeconds);
        }

        #endregion Cast Operators

        #region Plus Operators

        public static TimeSpanFloat operator +(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.Add(right);
        }
        public static TimeSpanFloat operator +(TimeSpanFloat left, TimeSpan right)
        {
            return left.Add(right);
        }
        public static TimeSpanFloat operator +(TimeSpanFloat left, float right)
        {
            return left.Add(right);
        }

        #endregion Plus Operators

        #region Minus Operators

        public static TimeSpanFloat operator -(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.Subtract(right);
        }
        public static TimeSpanFloat operator -(TimeSpanFloat left, TimeSpan right)
        {
            return left.Subtract(right);
        }
        public static TimeSpanFloat operator -(TimeSpanFloat left, float right)
        {
            return left.Subtract(right);
        }

        public static TimeSpanFloat operator *(TimeSpanFloat left, float right)
        {
            return left.Dot(right);
        }

        #endregion Minus Operators

        public static TimeSpanFloat operator /(TimeSpanFloat left, float right)
        {
            return left.Divide(right);
        }

        #region Compare Operators

        public static bool operator <(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator ==(TimeSpanFloat left, TimeSpanFloat right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TimeSpanFloat left, TimeSpanFloat right)
        {
            return !(left == right);
        }

        #endregion

        public static TimeSpanFloat FromDays(float days)
        {
            if (days <= 0f)
                return Empty;

            return new TimeSpanFloat(days);
        }

        public static TimeSpanFloat FromHours(float hours)
        {
            if (hours <= 0f)
                return Empty;

            return new TimeSpanFloat(hours * FROM_HOUR_TO_SECOND);
        }

        public static TimeSpanFloat FromMinutes(float minutes)
        {
            if (minutes <= 0f)
                return Empty;

            return new TimeSpanFloat(minutes * 60f);
        }

        public static TimeSpanFloat FromMilliseconds(int milliseconds)
        {
            if (milliseconds <= 0)
                return Empty;

            return new TimeSpanFloat(milliseconds * 100);
        }

        public readonly TimeSpanFloat TrimDays(out float days)
        {
            var hours = Hours;

            days = MathF.Floor(hours / 24f);

            return new TimeSpanFloat(hours - days * FROM_DAY_TO_SECOND);
        }

        public readonly TimeSpanFloat Add(TimeSpanFloat other)
        {
            return new TimeSpanFloat(Seconds + other.Seconds);
        }
        public readonly TimeSpanFloat Add(TimeSpan timeSpan)
        {
            return new TimeSpanFloat(Seconds + (float)timeSpan.TotalSeconds);
        }
        public readonly TimeSpanFloat Add(float seconds)
        {
            return new TimeSpanFloat(Seconds + seconds);
        }

        public readonly TimeSpanFloat Subtract(TimeSpanFloat other)
        {
            return new TimeSpanFloat(Seconds - other.Seconds);
        }
        public readonly TimeSpanFloat Subtract(TimeSpan timeSpan)
        {
            return new TimeSpanFloat(Seconds - (float)timeSpan.TotalSeconds);
        }
        public readonly TimeSpanFloat Subtract(float seconds)
        {
            return new TimeSpanFloat(Seconds - seconds);
        }

        public readonly TimeSpanFloat Dot(float multiplier)
        {
            if (multiplier <= 0f)
                return Empty;

            return new TimeSpanFloat(Seconds * multiplier);
        }

        public readonly TimeSpanFloat Divide(float divider)
        {
            if (divider == 0f)
                return default;

            return new TimeSpanFloat(MathF.Max(Seconds / divider, 0f));
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is TimeSpanFloat span && Equals(span);
        }

        public readonly bool Equals(TimeSpanFloat other)
        {
            return Seconds == other.Seconds;
        }

        public readonly bool NearlyEquals(TimeSpanFloat other, float? epsilon = null)
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

        public readonly int CompareTo(TimeSpanFloat other)
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

    public static class TimeSpanFloatExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat ToTimeSpanFloat(this TimeSpan timeSpan)
        {
            return new TimeSpanFloat((float)timeSpan.TotalSeconds);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat MillisecondsFloat(this int source)
        {
            return TimeSpanFloat.FromMilliseconds(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat Seconds(this float source)
        {
            return new TimeSpanFloat(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat Minutes(this float source)
        {
            return TimeSpanFloat.FromMinutes(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat Hours(this float source)
        {
            return TimeSpanFloat.FromHours(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpanFloat Days(this float source)
        {
            return TimeSpanFloat.FromDays(source);
        }
    }
}
