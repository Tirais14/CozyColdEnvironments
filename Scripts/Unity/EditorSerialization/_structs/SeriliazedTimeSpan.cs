using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public struct SeriliazedTimeSpan : IEditorSerialized<TimeSpan>
    {
        [SerializeField]
        [Tooltip("Do not select Tick, for this was specified SerializedTimeSpan.Ticks")]
        private TimeUnit timeUnit;

        [SerializeField]
        private float time;

        public readonly TimeSpan Output {
            get
            {
                if (timeUnit == TimeUnit.None
                    || 
                    timeUnit == TimeUnit.Day
                    || 
                    timeUnit == TimeUnit.Tick
                    )
                {
                    CCDebug.PrintError($"Invalid data. {nameof(timeUnit)}: {timeUnit}.");
                    return TimeSpan.Zero;
                } 

                return timeUnit switch
                {
                    TimeUnit.Millisecond => TimeSpan.FromMilliseconds(time),
                    TimeUnit.Second => TimeSpan.FromSeconds(time),
                    TimeUnit.Minute => TimeSpan.FromMinutes(time),
                    TimeUnit.Hour => TimeSpan.FromHours(time),
                    _ => TimeSpan.Zero
                };
            }
        }

        public static implicit operator TimeSpan(SeriliazedTimeSpan source)
        {
            return source.Output;
        }

        public struct Ticks : IEditorSerialized<TimeSpan>
        {
            [SerializeField]
            private long tickCount;

            public readonly TimeSpan Output => TimeSpan.FromTicks(tickCount);


            public static implicit operator TimeSpan(Ticks source)
            {
                return source.Output;
            }
        }
    }
}
