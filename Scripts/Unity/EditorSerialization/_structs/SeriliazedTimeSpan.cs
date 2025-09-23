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
        private float value;

        public readonly TimeSpan Output {
            get => timeUnit switch
            {
                TimeUnit.Millisecond => TimeSpan.FromMilliseconds(value),
                TimeUnit.Second => TimeSpan.FromSeconds(value),
                TimeUnit.Minute => TimeSpan.FromMinutes(value),
                TimeUnit.Hour => TimeSpan.FromHours(value),
                _ => throw new InvalidOperationException(timeUnit.ToString())
            };
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
