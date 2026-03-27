using CCEnvs.Attributes.Serialization;
using CCEnvs.Json;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("SnapshotProperty", "1f75ed73-b959-43be-823c-63c89f80918c")]
    public sealed class AnonymousSnapshotProperty : AnonymousSnapshotMember
    {
        [JsonProperty("capturedValue")]
        public object? CapturedValue { get; private set; }

        public AnonymousSnapshotProperty(FieldInfo fieldInfo)
            :
            base(fieldInfo)
        {
        }

        public AnonymousSnapshotProperty(PropertyInfo propInfo)
            :
            base(propInfo)
        {
        }

#nullable disable warnings
        private AnonymousSnapshotProperty()
        {
        }
#nullable disable warnings

        public override void ResetCapturedValue() => CapturedValue = null;

        public override void CaptureValueFrom(object target)
        {
            CapturedValue = GetValue(target);
        }

        public override void RestoreFromCaptured(object target)
        {
            SetValue(target, CapturedValue);
        }

        public override string ToString()
        {
            return base.ToString().Format(CapturedValue);
        }

        internal AnonymousSnapshotProperty SetUnderlyingValue(object? value)
        {
            CapturedValue = value;
            return this;
        }
    }
}
