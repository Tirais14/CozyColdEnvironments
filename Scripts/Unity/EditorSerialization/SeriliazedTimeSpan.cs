using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SeriliazedTimeSpan : IConvertibleCC<TimeSpan>
    {
        [SerializeField]
        [Tooltip("Do not select Tick, for this was specified SerializedTimeSpan.Ticks")]
        private TimeUnit timeUnit;

        [SerializeField]
        private float value;

        public static implicit operator TimeSpan(SeriliazedTimeSpan source)
        {
            return source.Convert();
        }

        public readonly TimeSpan Convert()
        {
            return timeUnit switch
            {
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds(value),
                TimeUnit.Second => TimeSpan.FromSeconds(value),
                TimeUnit.Minute => TimeSpan.FromMinutes(value),
                TimeUnit.Hour => TimeSpan.FromHours(value),
                _ => throw new InvalidOperationException(timeUnit.ToString())
            };
        }

        public struct Ticks : IConvertibleCC<TimeSpan>
        {
            [SerializeField]
            private long tickCount;

            public static implicit operator TimeSpan(Ticks source)
            {
                return source.Convert();
            }

            public TimeSpan Convert()
            {
                throw new NotImplementedException();
            }
        }
    }
}
