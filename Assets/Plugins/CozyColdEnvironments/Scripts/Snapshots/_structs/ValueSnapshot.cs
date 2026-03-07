using System;
using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [SerializationDescriptor("ValueSnapshot", "57858be4-477c-42bb-ae94-2959ec67138c")]
    public sealed record ValueSnapshot : Snapshot<object>
    {
        public ValueSnapshot()
        {
        }

        public ValueSnapshot(Snapshot<object> original) : base(original)
        {
        }

        public ValueSnapshot(object target) : base(target)
        {
        }

        protected override void OnRestore(ref object target)
        {
        }
    }

    [Serializable]
    public sealed record ValueSnapshot<T> : Snapshot<T>, ISnapshot<T>
    {
        [JsonProperty("value")]
        public T? Value { get; set; }

        public ValueSnapshot()
        {
        }

        public ValueSnapshot(T value)
            :
            base(value)
        {
        }

        public ValueSnapshot<T> WithValue(T value)
        {
            return new ValueSnapshot<T>(value);
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Value = target;
        }

        protected override void OnRestore(ref T target)
        {
        }

        protected override void OnReset()
        {
            base.OnReset();

            Value = default;
        }
    }
}
