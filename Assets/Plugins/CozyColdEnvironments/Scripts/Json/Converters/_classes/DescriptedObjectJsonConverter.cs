using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using CCEnvs.Diagnostics;
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
            {
                return true;
            }

            if (objectType.IsDefined<PolymorphSerializableAttribute>(inherit: true)
                ||
                objectType == TypeofCache<object>.Type
                ||
                objectType.IsAbstract || objectType.IsInterface
                )
            {
                return true;
            }

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

            //if (jToken is not JObject jObj)
            //    throw new JsonSerializationException($"Expected JObject. Current: {jToken.Type}: type: {objectType}");

            if (jToken is not JObject jObj
                ||
                !jObj.Properties().Any(prop => prop.Name == DESCRIPTOR_PROPERTY_NAME))
            {
                if (CCDebug.Instance.IsEnabled)
                    this.PrintLog("Is not JsonObject or not container descriptor property. Try to deserialize by other converters");

                var tSerializer = JsonSerializer.Create(serializer.GetSerializerSettings().ExcludeConverters(this));

                var result = tSerializer.Deserialize(jToken.CreateReader(), objectType);

                if (result is JToken && objectType.IsNotType<JToken>())
                    throw new InvalidOperationException($"Cannot deserialize abstract type by other converters. Type: {objectType}");

                return result;
            }

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
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName(DESCRIPTOR_PROPERTY_NAME);
                serializer.Serialize(writer, descriptor);
                writer.WritePropertyName("value");

                var tSerializer = JsonSerializer.Create(serializer.GetSerializerSettings().ExcludeConverters(this));
                tSerializer.Serialize(writer, value);
                writer.WriteEndObject();
            }
            //else
            //    throw new NotImplementedException(objType.ToString());
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
