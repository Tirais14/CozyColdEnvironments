using System;
using System.Reflection;
using CCEnvs.Reflection;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#nullable enable
namespace CCEnvs.Json
{
    public class DescriptedObjectJsonConverter : JsonConverter
    {
        public const string DESCRIPTOR_PROPERTY_NAME = "$typeDescriptor";

        public override bool CanConvert(Type objectType)
        {
            if (TypeSerializationHelper.TypeDescriptors.ContainsKey(objectType))
                return true;

            if (objectType.GetCustomAttribute<PolymorphSerializableAttribute>(inherit: true) is not null)
                return true;

            return false;
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
                throw new JsonSerializationException($"Expected JObject. Current: {jToken.Type}: type: {objectType}");

            if (jObj.Property(DESCRIPTOR_PROPERTY_NAME).IsNull(out var descriptorProp))
                return OnDescriptorPropertyNotFound(jObj, objectType, serializer);

            var descriptor = descriptorProp.Value.ToObject<TypeSerializationDescriptor>(serializer);

            if (descriptor == default
                ||
                !TypeSerializationHelper.DescriptedTypes.TryGetValue(descriptor, out var resultType))
            {
                return OnDescriptorNotFound(jObj, objectType, serializer);
            }

            jObj.Remove(DESCRIPTOR_PROPERTY_NAME);

            var readed = OnReadJson(jObj, resultType, serializer);

            return readed;
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

                JsonSerializerInternalWriterHelper.WriteObjectBody(
                    writer,
                    jObjContract,
                    serializer,
                    value
                    );

                writer.WriteEndObject();
            }
            //else if (contract is JsonPrimitiveContract jPrimitiveContract)
            //{
            //    //var serializerSettings = serializer.GetSerializerSettings()
            //    //    .ExcludeConverters(typeof(DescriptedObjectJsonConverter));

            //    //var primitiveSerializer = JsonSerializer.Create(serializerSettings);
            //    writer.WriteStartObject();
            //    writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);
            //    serializer.Serialize(writer, descriptor);
            //    writer.WritePropertyName("value");
            //    writer.WriteValue(value);
            //    writer.WriteEndObject();
            //}
            else
                throw new NotImplementedException(objType.ToString());
        }

        protected virtual object? OnDescriptorPropertyNotFound(
            JObject jObj,
            Type objType,
            JsonSerializer serializer
            )
        {
            throw new JsonSerializationException($"Object: {objType}, property: {DESCRIPTOR_PROPERTY_NAME} value is null");
        }

        protected virtual object? OnDescriptorNotFound(
            JObject jObj,
            Type objType,
            JsonSerializer serializer
            )
        {
            throw new JsonSerializationException($"Object: {objType} hasn't {nameof(TypeSerializationDescriptor)}");
        }

        protected virtual object? OnReadJson(
            JObject jObj,
            Type objType,
            JsonSerializer serializer
            )
        {
            var instance = JsonSerializerInternalReaderHelper.CreateNewObject(objType, jObj.CreateReader(), serializer);
            serializer.Populate(jObj.CreateReader(), instance);

            return instance;
        }
    }
}
