using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Json
{
    public class ByDescriptorJsonConverter : JsonConverter
    {
        public const string DESCRIPTOR_PROPERTY_NAME = "$typeDescriptor";

        public override bool CanConvert(Type objectType)
        {
            return TypeSerializationHelper.TypeDescriptors.ContainsKey(objectType)
                   ||
                   objectType.GetCustomAttribute<PolymorphSerializableAttribute>(inherit: true) is not null;
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

            var jToken = JToken.Load(reader);

            if (jToken is not JObject jObj)
                throw new JsonSerializationException("Expected JObject");

            if (jObj.Property(DESCRIPTOR_PROPERTY_NAME).IsNull(out var descriptorProp))
                throw new JsonSerializationException($"Object: {objectType}, property: {DESCRIPTOR_PROPERTY_NAME} value is null");

            var descriptor = descriptorProp.Value.ToObject<TypeSerializationDescriptor>(serializer);

            if (descriptor == default)
                throw new JsonSerializationException($"Object: {objectType}, {nameof(TypeSerializationDescriptor)} value not found");

            if (!TypeSerializationHelper.DescriptedTypes.TryGetValue(descriptor, out var resultType))
                throw new JsonSerializationException($"Type: {objectType} hasn't {nameof(TypeSerializationDescriptor)}");

            jObj.Remove(DESCRIPTOR_PROPERTY_NAME);

            var jContract = serializer.ContractResolver.ResolveContract(resultType);

            if (jContract is not JsonObjectContract jObjContract)
                throw new JsonSerializationException("Expected JsonObjectContract");

            object instance = JsonSerializerInternalReaderHelper.CreateNewObject(resultType, jObj.CreateReader(), serializer);

            serializer.Populate(jObj.CreateReader(), instance);

            return instance;
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer
            )
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            var objType = value.GetType();

            if (!TypeSerializationHelper.TryGetTypeSerializationDescriptor(objType, out var descriptor))
                throw new JsonSerializationException($"Type: {value.GetType()} hasn't type descriptor");

            serializer.TypeNameHandling = TypeNameHandling.None;

            var contract = serializer.ContractResolver.ResolveContract(objType);

            if (contract is JsonObjectContract jObjContract)
            {
                writer.WriteStartObject();

                writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);

                serializer.Serialize(writer, descriptor);

                JsonSerializerInternalWriterhelper.WriteObjectBody(
                    writer,
                    jObjContract,
                    serializer,
                    value
                    );

                writer.WriteEndObject();
            }
            else
                throw new NotImplementedException(objType.ToString());
        }
    }
}
