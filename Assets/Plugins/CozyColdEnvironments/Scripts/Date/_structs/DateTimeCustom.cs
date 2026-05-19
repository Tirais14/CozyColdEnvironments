using CCEnvs.Collections;
using CCEnvs.Pools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public readonly struct DateTimeCustom : IEquatable<DateTimeCustom>, IComparable<DateTimeCustom>
    {
        public readonly static Lazy<StructuralArray<int>> defaultCalendar = new(
            () =>
            {
                var arr = new int[12];
                Array.Fill(arr, 30);

                return new StructuralArray<int>(arr, isReadOnly: true, forceCacheHashCode: true);
            });

        [JsonProperty("calendar")]
        private readonly StructuralArray<int> calendar;

        [JsonProperty("year")]
        public readonly int Year { get; }

        [JsonProperty("month")]
        public readonly int Month { get; }

        [JsonProperty("day")]
        public readonly int Day { get; }


        [JsonProperty("time")]
        public readonly TimeSpanLight Time { get; }

        [JsonConstructor]
        public DateTimeCustom(
            int year,
            int month,
            int day,
            TimeSpanLight time,
            StructuralArray<int> calendar = default
            )
        {
            if (!isValidCalendar(calendar))
                calendar = defaultCalendar.Value;

            this.calendar = calendar;

            if (time.Hours >= 24f)
            {
                OffsetDate(
                    year,
                    month,
                    day,
                    time,
                    calendar,
                    out year,
                    out month,
                    out day,
                    out time
                    );
            }

            Year = year;
            Month = month;
            Day = day;
            Time = time;

            static bool isValidCalendar(StructuralArray<int> calendar)
            {
                if (!calendar.IsInitialized || calendar.Length == 0)
                    return false;

                for (int i = 0; i < calendar.Length; i++)
                    if (calendar[i] < 1)
                        return false;

                return true;
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

        public static DateTimeCustom operator +(DateTimeCustom left, TimeSpanLight right)
        {
            return left.Add(right);
        }

        public static void OffsetDate(
            int year,
            int month,
            int day,
            TimeSpanLight time,
            StructuralArray<int> calendar,
            out int newYear,
            out int newMonth,
            out int newDay,
            out TimeSpanLight newTime
            )
        {
            int daysToAdd = (int)MathF.Floor(time.Seconds / TimeSpanLight.FROM_DAY_TO_SECOND);
            float remainingSeconds = time.Seconds - (daysToAdd * TimeSpanLight.FROM_DAY_TO_SECOND);

            int currentDay = day + daysToAdd;
            int currentMonth = month;
            int currentYear = year;

            while (currentDay > calendar[currentMonth - 1])
            {
                currentDay -= calendar[currentMonth - 1];
                currentMonth++;

                if (currentMonth > calendar.Length)
                {
                    currentMonth = 1;
                    currentYear++;
                }
            }

            newYear = currentYear;
            newMonth = currentMonth;
            newDay = currentDay;
            newTime = new TimeSpanLight(remainingSeconds);
        }

        public readonly DateTimeCustom Add(TimeSpanLight otherTime)
        {
            return new DateTimeCustom(Year, Month, Day, Time + otherTime, calendar);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DateTimeCustom time && Equals(time);
        }

        public readonly bool Equals(DateTimeCustom other)
        {
            return calendar == other.calendar
                   &&
                   Year == other.Year 
                   &&
                   Month == other.Month
                   &&
                   Day == other.Day
                   &&
                   EqualityComparer<TimeSpanLight>.Default.Equals(Time, other.Time);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(calendar, Year, Month, Day, Time);
        }

        public readonly string ToString(TimeSpanLight.StringFormat timeFormat)
        {
            using var sb = StringBuilderPool.Shared.Get();

            if (Day < 10 && Day > -10)
                sb.Value.Append('0');

            sb.Value.Append(Day);
            sb.Value.Append('.');

            if (Month < 10 && Month > -10)
                sb.Value.Append('0');

            sb.Value.Append(Month);
            sb.Value.Append('.');
            sb.Value.Append(Year);
            sb.Value.Append(" (");
            sb.Value.Append(Time.ToString(timeFormat));
            sb.Value.Append(')');

            return sb.Value.ToString();
        }

        public readonly override string ToString()
        {
            return ToString(TimeSpanLight.StringFormat.Default);
        }

        public readonly int CompareTo(DateTimeCustom other)
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
