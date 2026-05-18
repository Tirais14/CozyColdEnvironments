using CCEnvs.Pools;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Dates
{
    public readonly struct DateTimeCustom : IEquatable<DateTimeCustom>, IComparable<DateTimeCustom>
    {
        private readonly int[] calendar;

        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        public TimeSpanLight Time { get; }

        public DateTimeCustom(
            int year,
            int month,
            int day,
            TimeSpanLight time,
            int[]? calendar = null
            )
        {
            Year = year;
            Month = month;
            Day = day;
            Time = time;

            if (calendar is null || calendar.Length == 0 || isInvalidCalendar(calendar))
            {
                calendar = new int[12];
                Array.Fill(calendar, 30);
            }

            this.calendar = calendar;

            static bool isInvalidCalendar(int[] calendar)
            {
                for (int i = 0; i < calendar.Length; i++)
                {
                    if (calendar[i] < 1)
                        return true;
                }

                return false;
            }
        }

        public static bool operator ==(DateTimeCustom left, DateTimeCustom right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateTimeCustom left, DateTimeCustom right)
        {
            return !(left == right);
        }

        public static bool operator <(DateTimeCustom left, DateTimeCustom right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(DateTimeCustom left, DateTimeCustom right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(DateTimeCustom left, DateTimeCustom right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(DateTimeCustom left, DateTimeCustom right)
        {
            return left.CompareTo(right) >= 0;
        }

        public DateTimeCustom Add(TimeSpanLight otherTime)
        {
            var resultTime = Time + otherTime;

            if (resultTime.Hours >= 24f)
            {
                var additionalDays = MathF.Floor(resultTime.Hours / 24f);

                resultTime -= TimeSpanLight.FromHours(additionalDays * 24f);

                var day = Day + 1;

                if (day > calendar[Month])
                {
                    var month 
                }
            }
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DateTimeCustom time && Equals(time);
        }

        public readonly bool Equals(DateTimeCustom other)
        {
            return Year == other.Year 
                   &&
                   Month == other.Month
                   &&
                   Day == other.Day
                   &&
                   EqualityComparer<TimeSpanLight>.Default.Equals(Time, other.Time);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Year, Month, Day, Time);
        }

        public override string ToString()
        {
            using var sb = StringBuilderPool.Shared.Get();

            sb.Value.Append(Day);
            sb.Value.Append('.');
            sb.Value.Append(Month);
            sb.Value.Append('.');
            sb.Value.Append(Year);
            sb.Value.Append(" (");
            sb.Value.Append(Time);
            sb.Value.Append(')');

            return sb.Value.ToString();
        }

        public int CompareTo(DateTimeCustom other)
        {
            int comp;

            comp = Year.CompareTo(other.Year);

            if (Year.CompareTo(other.Year) != 0)
                return comp;

            comp = Month.CompareTo(other.Month);

            if (comp != 0)
                return comp;

            comp = Day.CompareTo(other.Day);

            if (comp != 0)
                return comp;

            return 0;
        }
    }
}
