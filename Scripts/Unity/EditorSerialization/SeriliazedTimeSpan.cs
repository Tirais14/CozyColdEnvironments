using NaughtyAttributes;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.EditorSerialization
{
    public record SeriliazedTimeSpan : IConvertibleCC<TimeSpan>
    {
        [SerializeField]
        private TimeUnit timeUnit;

        [ShowIf(nameof(timeUnit), TimeUnit.Tick)]
        private long ticks;

        [ShowIf(nameof(timeUnit), TimeUnit.Millisecond)]
        private double milliseconds;

        [ShowIf(nameof(timeUnit), TimeUnit.Second)]
        private double seconds;

        [ShowIf(nameof(timeUnit), TimeUnit.Minute)]
        private double minutes;

        [ShowIf(nameof(timeUnit), TimeUnit.Hour)]
        private double hours;

        public TimeSpan Convert()
        {
            return timeUnit switch
            {
                TimeUnit.Tick => TimeSpan.FromTicks(ticks),
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds(milliseconds),
                TimeUnit.Second => TimeSpan.FromSeconds(seconds),
                TimeUnit.Minute => TimeSpan.FromMinutes(minutes),
                TimeUnit.Hour => TimeSpan.FromHours(hours),
                _ => throw new InvalidOperationException(timeUnit.ToString())
            };

        }
    }
}
