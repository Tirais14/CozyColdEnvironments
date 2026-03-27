using CCEnvs.Attributes.Serialization;
using CCEnvs.Reflection;
using CCEnvs.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        protected AnonymousSnapshotMember(FieldInfo fieldInfo)
        {
            Guard.IsNotNull(fieldInfo, nameof(fieldInfo));

            MemberInfo = fieldInfo;
        }

        protected AnonymousSnapshotMember(PropertyInfo propInfo)
        {
            Guard.IsNotNull(propInfo, nameof(propInfo));

            if (!propInfo.CanRead)
                throw new ArgumentException($"Property must be readable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            if (!propInfo.CanWrite)
                throw new ArgumentException($"Property must be writable. DeclaringType: {propInfo.DeclaringType.Name}; name: {propInfo.Name}");

            MemberInfo = propInfo;
        }

#nullable disable warnings
        protected AnonymousSnapshotMember()
        {
        }
#nullable enable warnings

        public object? GetValue(object target)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
                return FieldInfo.GetValue(target);
            else
                return PropInfo!.GetValue(target);
        }

        public abstract void ResetCapturedValue();

        public abstract void CaptureValueFrom(object target);

        public abstract void RestoreFromCaptured(object target);

        internal virtual void SetValue(object target, object? value)
        {
            CC.Guard.IsNotNullTarget(target);

            if (FieldInfo is not null)
            {
                value = convertIfNotType(FieldInfo.FieldType, value);
                FieldInfo.SetValue(target, value);
            }
            else
            {
                value = convertIfNotType(PropInfo!.PropertyType, value);
                PropInfo!.SetValue(target, value);
            }

            static object? convertIfNotType(Type targetType, object? value)
            {
                if (value.IsNull())
                    return value;

                if (targetType.IsPrimitiveNumber())
                    return TypeHelper.ConvertNumber(value, targetType);

                ////TODO: Fix bug where JsonObject inject in field
                //if (value is JObject jObj)
                //    return jObj.ToObject(targetType, JsonSerializer.Create(CC.SerializerSettings));

                if (!targetType.GetType().IsAssignableFrom(value.GetType()))
                    value = value.MutateType(targetType);

                return value;
            }
        }
    }
}
