using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class PolymorphJsonConverter<T> : JsonConverter<T>
    {
        public override T? ReadJson(
            JsonReader reader,
            Type objectType,
            T? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException();

            var jObj = JObject.Load(reader);
            JProperty typeProp = jObj.Property("$type") ?? throw new JsonSerializationException("Missing \"$type\" property");
            string typeReference = typeProp.Value.ToString();
            var actualType = Type.GetType(typeReference, throwOnError: true);

            //T inst;
            //try
            //{
            //    inst = actualType.Reflect().CreateInstance<T>();
            //}
            //catch (Exception ex)
            //{
            //    throw new JsonSerializationException($"Type \"{actualType}\" not supports constructor with parameters for now", ex);
            //}

            //JsonConverterHelper.Populate(inst!, jObj);

            return (T)JsonConverterHelper.CreateInstance(actualType, jObj.Properties().ToArray(), serializer);
        }

        public override void WriteJson(
            JsonWriter writer,
            T? value,
            JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue(value.GetType().GetTypeReference());

            Type valueType = value.GetType();
            var jPropInfos = JsonConverterHelper.ResolveJsonPropertyInfos(valueType, serializer.GetSerializerSettings());
            JsonSerializerSettings serializerSettings = serializer.GetSerializerSettings();
            foreach (var jPropInfo in jPropInfos)
            {
                if (!jPropInfo.ShouldSerialize)
                    continue;

                if (jPropInfo.Get is null || !jPropInfo.Get(value).Let(out object? propValue))
                {
                    if (serializer.NullValueHandling == NullValueHandling.Ignore)
                        continue;

                    writer.WritePropertyName(jPropInfo.Name);
                    writer.WriteNull();
                    continue;
                }

                writer.WritePropertyName(jPropInfo.Name);
                serializer.Serialize(writer, propValue);
            }

            writer.WriteEndObject();
        }
    }
}
