using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public sealed class SnapshotProperty
    {
        [JsonIgnore]
        public FieldInfo? FieldInfo => MemberInfo as FieldInfo;

        [JsonIgnore]
        public PropertyInfo? PropInfo => MemberInfo as PropertyInfo;

        [JsonProperty("memberInfo")]
        public MemberInfo MemberInfo { get; private set; }

        [JsonProperty("capturedValue")]
        public object? CapturedValue { get; private set; }

        public SnapshotProperty(FieldInfo fieldInfo)
        {
            Guard.IsNotNull(fieldInfo, nameof(fieldInfo));

            MemberInfo = fieldInfo;
        }

        public SnapshotProperty(PropertyInfo propInfo)
        {
            Guard.IsNotNull(propInfo, nameof(propInfo));

            if (!propInfo.CanRead)
                throw new ArgumentException($"Property must be readable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            if (!propInfo.CanWrite)
                throw new ArgumentException($"Property must be writable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            MemberInfo = propInfo;
        }

        public void SetValue(object target, object? value)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
                FieldInfo.SetValue(target, value);
            else
                PropInfo!.SetValue(target, value);
        }

        public object GetValue(object target)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
                return FieldInfo.GetValue(target);
            else
                return PropInfo!.GetValue(target);
        }

        internal void CaptureValueFrom(object target)
        {
            CapturedValue = GetValue(target);
        }

        internal void ResetCapturedValue() => CapturedValue = null;
    }
}
