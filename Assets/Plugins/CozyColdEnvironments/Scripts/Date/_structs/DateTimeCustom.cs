using CCEnvs.Collections;
using CCEnvs.Pools;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

#nullable enable
namespace CCEnvs.Dates
{
    [Serializable, DataContract]
    public readonly struct DateTimeCustom : IEquatable<DateTimeCustom>, IComparable<DateTimeCustom>
    {
        public static DateTimeCustom Default => new();

        private readonly static Lazy<StructuralArray<int>> defaultCalendar = new(
            () =>
            {
                var arr = new int[12];
                Array.Fill(arr, 30);

                return new StructuralArray<int>(arr, isReadOnly: true, forceCacheHashCode: true);
            });

        [DataMember(Name = "calendar")]
        private readonly StructuralArray<int> calendar;

        [DataMember(Name = "year")]
        public readonly int Year { get; }

        [DataMember(Name = "month")]
        public readonly int Month { get; }

        [DataMember(Name = "day")]
        public readonly int Day { get; }


        [DataMember(Name = "time")]
        public readonly TimeSpanFloat Time { get; }

#if JSON_NET
        [JsonConstructor]
#endif
        public DateTimeCustom(
            int year,
            int month,
            int day,
            TimeSpanFloat time,
            StructuralArray<int> calendar = default
            )
        {
            if (!isValidCalendar(calendar))
                calendar = defaultCalendar.Value;

            this.calendar = calendar;

            if (time.Hours >= 24f)
            {
                AddDateOffset(
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

        public static DateTimeCustom operator +(DateTimeCustom left, TimeSpanFloat right)
        {
            return left.Add(right);
        }

        public static DateTimeCustom operator -(DateTimeCustom left, TimeSpanFloat right)
        {
            return left.Minus(right);
        }

        public static void AddDateOffset(
            int year,
            int month,
            int day,
            TimeSpanFloat time,
            StructuralArray<int> calendar,
            out int newYear,
            out int newMonth,
            out int newDay,
            out TimeSpanFloat newTime
            )
        {
            int daysToAdd = (int)MathF.Floor(time.Seconds / TimeSpanFloat.FROM_DAY_TO_SECOND);
            float remainingSeconds = time.Seconds - (daysToAdd * TimeSpanFloat.FROM_DAY_TO_SECOND);

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
            newTime = new TimeSpanFloat(remainingSeconds);
        }

        public static void RemoveDateOffset(
            int year,
            int month,
            int day,
            TimeSpanFloat time,
            StructuralArray<int> calendar,
            out int newYear,
            out int newMonth,
            out int newDay,
            out TimeSpanFloat newTime
            )
        {
            float totalSeconds = time.Seconds;
            int daysToRemove = (int)MathF.Floor(totalSeconds / TimeSpanFloat.FROM_DAY_TO_SECOND);
            float secRemainder = totalSeconds - (daysToRemove * TimeSpanFloat.FROM_DAY_TO_SECOND);

            int currentDay = day - daysToRemove;
            int currentMonth = month;
            int currentYear = year;

            // 1. Откатываем целые дни (заём у предыдущих месяцев/лет)
            while (currentDay <= 0)
            {
                currentMonth--;
                if (currentMonth <= 0)
                {
                    currentMonth = calendar.Length;
                    currentYear--;
                }
                currentDay += calendar[currentMonth - 1];
            }

            if (secRemainder > 0)
            {
                currentDay--;
                if (currentDay <= 0)
                {
                    currentMonth--;
                    if (currentMonth <= 0)
                    {
                        currentMonth = calendar.Length;
                        currentYear--;
                    }
                    currentDay += calendar[currentMonth - 1];
                }
                newTime = new TimeSpanFloat(TimeSpanFloat.FROM_DAY_TO_SECOND - secRemainder);
            }
            else
            {
                newTime = TimeSpanFloat.Empty;
            }

            newYear = currentYear;
            newMonth = currentMonth;
            newDay = currentDay;
        }

        public readonly DateTimeCustom Add(TimeSpanFloat otherTime)
        {
            return new DateTimeCustom(Year, Month, Day, Time + otherTime, calendar);
        }

        public readonly DateTimeCustom Minus(TimeSpanFloat otherTime)
        {
            float diff = Time.Seconds - otherTime.Seconds;

            if (diff >= 0)
                return new DateTimeCustom(Year, Month, Day, new TimeSpanFloat(diff), calendar);

            RemoveDateOffset(
                Year, Month, Day,
                new TimeSpanFloat(TimeSpanFloat.FROM_DAY_TO_SECOND + MathF.Abs(diff)),
                calendar,
                out int ny, out int nm, out int nd, out TimeSpanFloat nt);

            return new DateTimeCustom(ny, nm, nd, nt, calendar);
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
                   Time == other.Time;
        }

        public readonly bool NearlyEquals(DateTimeCustom other, float? epsilon = null)
        {
            return calendar == other.calendar
                   &&
                   Year == other.Year
                   &&
                   Month == other.Month
                   &&
                   Day == other.Day
                   &&
                   Time.NearlyEquals(other.Time, epsilon);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(calendar, Year, Month, Day, Time);
        }

        public readonly string ToString(TimeSpanFloat.StringFormat timeFormat)
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
            return ToString(TimeSpanFloat.StringFormat.Default);
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
