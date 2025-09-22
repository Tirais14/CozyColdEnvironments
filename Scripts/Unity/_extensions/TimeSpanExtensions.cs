using System;

#nullable enable
namespace CCEnvs.Unity
{
    public static class TimeSpanExtensions
    {
        public static float TotalMillisecondsF(this TimeSpan source)
        {
            return (float)source.TotalMilliseconds;
        }

        public static float TotalSecondsF(this TimeSpan source)
        {
            return (float)source.TotalSeconds;
        }

        public static float TotalMinutesF(this TimeSpan source)
        {
            return (float)source.TotalMinutes;
        }

        public static float TotalHoursF(this TimeSpan source)
        {
            return (float)source.TotalHours;
        }
    }
}
