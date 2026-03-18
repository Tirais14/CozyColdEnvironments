using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("SnapshotProperty", "1f75ed73-b959-43be-823c-63c89f80918c")]
    public sealed class AnonymousSnapshotProperty :AnonymousSnapshotMember
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

        internal override void CaptureValueFrom(object target)
        {
            CapturedValue = GetValue(target);
        }

        internal override void ResetCapturedValue() => CapturedValue = null;

        internal override void RestoreFromCaptured(object target)
        {
            SetValue(target, CapturedValue);
        }
    }
}
