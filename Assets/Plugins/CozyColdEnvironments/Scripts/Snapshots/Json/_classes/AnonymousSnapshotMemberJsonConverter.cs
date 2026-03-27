using CCEnvs.Json;
using CCEnvs.Reflection;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots.Json
{
    public class AnonymousSnapshotMemberJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsType<AnonymousSnapshotMember>();
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObj = JObject.Load(reader);

            var memberInfoProp = jObj.Property(nameof(AnonymousSnapshotMember.MemberInfo), StringComparison.OrdinalIgnoreCase)
                ??
                throw new JsonSerializationException("Cannot find member info prop");

            var memberInfo = serializer.Deserialize<MemberInfo>(memberInfoProp.Value.CreateReader());

            if (memberInfo.IsNull())
                throw new JsonSerializationException("Cannot deserialize member info");

            var typeDescriptorProp = jObj.Property(DescriptedObjectJsonConverter.DESCRIPTOR_PROPERTY_NAME, StringComparison.OrdinalIgnoreCase)
                ??
                throw new JsonSerializationException("Cannot find type descriptor property");

            var typeDescriptor = serializer.Deserialize<TypeSerializationDescriptor>(typeDescriptorProp.Value.CreateReader());

            if (typeDescriptor.IsDefault())
                throw new JsonSerializationException("Cannot deserialize type serialization descriptor");

            objectType = TypeSerializationHelper.DescriptedTypes[typeDescriptor];

            var fieldInfo = memberInfo as FieldInfo;
            var propInfo = memberInfo as PropertyInfo;

            if (objectType.IsType<AnonymousSnapshotProperty>())
            {
                var capturedValueProp = jObj.Property(nameof(AnonymousSnapshotProperty.CapturedValue), StringComparison.OrdinalIgnoreCase)
                    ??
                    throw new JsonSerializationException("Cannot find captured value prop");

                var memberUnderlyingType = (fieldInfo is not null ? fieldInfo.FieldType : propInfo?.PropertyType)
                    ??
                    throw new JsonSerializationException("Cannot resolve underlying type");

                var underlyingValue = serializer.Deserialize(capturedValueProp.Value.CreateReader(), memberUnderlyingType);

                if (fieldInfo is not null)
                    return new AnonymousSnapshotProperty(fieldInfo).SetUnderlyingValue(underlyingValue);
                else if (propInfo is not null)
                    return new AnonymousSnapshotProperty(propInfo).SetUnderlyingValue(underlyingValue);
            }
            else if (objectType.IsType<AnonymousSnapshotCompositePart>())
            {
                var underlyingSnapshotProp = jObj.Property(nameof(AnonymousSnapshotCompositePart.Snapshot), StringComparison.OrdinalIgnoreCase)
                    ??
                    throw new JsonSerializationException($"Cannot find snapshot prop");

                var underlyingSnapshot = serializer.Deserialize<ISnapshot>(underlyingSnapshotProp.Value.CreateReader());

                CC.Guard.IsNotNull(underlyingSnapshot, nameof(underlyingSnapshot));

                if (fieldInfo is not null)
                    return new AnonymousSnapshotCompositePart(fieldInfo, underlyingSnapshot);
                else if (propInfo is not null)
                    return new AnonymousSnapshotCompositePart(propInfo, underlyingSnapshot);
            }

            throw new InvalidOperationException(objectType.ToString());
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            var tSerializer = JsonSerializer.Create(serializer.GetSerializerSettings().ExcludeConverters(this));
            tSerializer.Serialize(writer, value);
        }
    }
}
