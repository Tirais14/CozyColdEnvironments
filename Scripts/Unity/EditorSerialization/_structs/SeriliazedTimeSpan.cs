using CCEnvs.Diagnostics;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.EditorSerialization
{
    [Serializable]
    public class SeriliazedTimeSpan 
        : IEditorSerialized<TimeSpan>,
        ITransformable<TimeSpan>,
        ISerializationCallbackReceiver
    {
        [SerializeField, Min(0)]
        private int ticks;

        [SerializeField, Min(0)]
        private int milliseconds;

        [SerializeField, Min(0f)]
        private float seconds;

        [SerializeField, Min(0f)]
        private float minutes;

        [SerializeField, Min(0f)]
        private float hours;

        [SerializeField, Min(0f)]
        private float days;

        public TimeSpan Output { get; private set; }

        public static implicit operator TimeSpan(SeriliazedTimeSpan source)
        {
            return source.Output;
        }

        TimeSpan ITransformable<TimeSpan>.DoTransform() => Output;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Output = TimeSpan.FromTicks(ticks) 
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
