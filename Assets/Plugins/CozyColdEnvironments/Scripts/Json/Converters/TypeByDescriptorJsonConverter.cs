using CCEnvs.Attributes.Serialization;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Json
{
    public class TypeByDescriptorJsonConverter : JsonConverter
    {
        public const string DESCRIPTOR_PROPERTY_NAME = "$typeDescriptor";

        public override bool CanConvert(Type objectType)
        {
            return !objectType.IsGenericType
                    &&
                    objectType.GetCustomAttribute<TypeSerializationDescriptorAttribute>() is not null;
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
                throw new JsonSerializationException($"Property: {DESCRIPTOR_PROPERTY_NAME} value is null");

            var descriptor = descriptorProp.Value.ToObject<TypeSerializationDescriptor>(serializer);

            if (descriptor == default)
                throw new JsonSerializationException($"{nameof(TypeSerializationDescriptor)} value not found");

            if (!TypeSerializationHelper.DescriptedTypes.TryGetValue(descriptor, out var resultType))
                throw new JsonSerializationException($"Type: {objectType} hasn't {nameof(TypeSerializationDescriptor)}");

            jObj.Remove(DESCRIPTOR_PROPERTY_NAME);

            var jContract = serializer.ContractResolver.ResolveContract(resultType);

            if (jContract is not JsonObjectContract jObjContract)
                throw new JsonSerializationException("Expected JsonObjectContract");

            object instance = JsonConverterHelper.CreateNewObject(resultType, jObj.CreateReader(), serializer);

            serializer.Populate(jObj.CreateReader(), instance);

            return instance;

            //static bool TryFallback(
            //    JObject jObj,
            //    Type objectType,
            //    object? existingValue,
            //    JsonSerializer serializer,
            //    out object? result
            //    )
            //{
            //    if (jObj["$type"] == null)
            //    {
            //        result = null;
            //        return false;
            //    }

            //    result = new PolymorphJsonConverter<object>().ReadJson(jObj.CreateReader(), objectType, existingValue, serializer);
            //    return true;
            //}
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

            var valueType = value.GetType();

            if (!TypeSerializationHelper.TryGetTypeSerializationDescriptor(valueType, out var descriptorAtt))
                throw new JsonSerializationException($"Type: {value.GetType()} hasn't type descriptor");

            serializer.TypeNameHandling = TypeNameHandling.None;

            writer.WriteStartObject();

            writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);

            serializer.Serialize(writer, descriptorAtt);

            var jPropInfos = JsonConverterHelper.ResolveJsonPropertyInfos(value.GetType(), serializer.GetSerializerSettings());

            foreach (var jPropInfo in jPropInfos)
            {
                if (!jPropInfo.ShouldSerialize)
                    continue;

                writer.WritePropertyName(jPropInfo.Name);

                serializer.Serialize(writer, jPropInfo?.Get?.Invoke(value));
            }

            writer.WriteEndObject();
        }
    }
}
