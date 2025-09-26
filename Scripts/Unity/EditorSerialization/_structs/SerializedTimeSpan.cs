using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SerializedTimeSpan 
        : IEditorSerialized<TimeSpan>,
        ITransformable<TimeSpan>,
        ISerializationCallbackReceiver
    {
        [SerializeField, Min(0)]
        private long ticks;

        [SerializeField, Min(0)]
        private int milliseconds;

        [SerializeField, Min(0f)]
        private double seconds;

        [SerializeField, Min(0f)]
        private double minutes;

        [SerializeField, Min(0f)]
        private double hours;

        [SerializeField, Min(0f)]
        private double days;

        public SerializedTimeSpan()
        {
        }

        public SerializedTimeSpan(TimeSpan defaultOutput)
        {
            ticks = defaultOutput.Ticks;
            milliseconds = defaultOutput.Milliseconds;
            seconds = defaultOutput.TotalSeconds;
            minutes = defaultOutput.TotalMinutes;
            hours = defaultOutput.TotalHours;
            days = defaultOutput.TotalDays;

            Value = defaultOutput;
        }

        public TimeSpan Value { get; private set; }

        public static implicit operator TimeSpan(SerializedTimeSpan source)
        {
            return source.Value;
        }

        TimeSpan ITransformable<TimeSpan>.DoTransform() => Value;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Value = TimeSpan.FromTicks(ticks) 
                     +
                     TimeSpan.FromMilliseconds(milliseconds)
                     + 
                     TimeSpan.FromSeconds(seconds)
                     + 
                     TimeSpan.FromMinutes(minutes)
                     + 
                     TimeSpan.FromHours(hours)
                     + 
                     TimeSpan.FromDays(days);
        }
    }
}
