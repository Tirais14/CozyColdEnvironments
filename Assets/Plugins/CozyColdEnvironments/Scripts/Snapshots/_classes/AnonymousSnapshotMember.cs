using CCEnvs.Attributes.Serialization;
using CCEnvs.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable, PolymorphSerializable, SerializationDescriptor("AnonymousSnapshotMember", "45e4fc3b-62b0-4c5f-b39e-2adb6f303d91")]
    public abstract class AnonymousSnapshotMember
    {
        [JsonIgnore]
        public FieldInfo? FieldInfo => MemberInfo as FieldInfo;

        [JsonIgnore]
        public PropertyInfo? PropInfo => MemberInfo as PropertyInfo;

        [JsonProperty("memberInfo")]
        public MemberInfo MemberInfo { get; private set; }

        public AnonymousSnapshotMember(FieldInfo fieldInfo)
        {
            Guard.IsNotNull(fieldInfo, nameof(fieldInfo));

            MemberInfo = fieldInfo;
        }

        public AnonymousSnapshotMember(PropertyInfo propInfo)
        {
            Guard.IsNotNull(propInfo, nameof(propInfo));

            if (!propInfo.CanRead)
                throw new ArgumentException($"Property must be readable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            if (!propInfo.CanWrite)
                throw new ArgumentException($"Property must be writable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            MemberInfo = propInfo;
        }

        public object? GetValue(object target)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
                return FieldInfo.GetValue(target);
            else
                return PropInfo!.GetValue(target);
        }

        internal virtual void SetValue(object target, object? value)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
                FieldInfo.SetValue(target, value);
            else
                PropInfo!.SetValue(target, value);
        }

        internal abstract void CaptureValueFrom(object target);

        internal abstract void ResetCapturedValue();

        internal abstract void RestoreFromCaptured(object target);
    }
}
