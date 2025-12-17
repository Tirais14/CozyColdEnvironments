using CCEnvs.Reflection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class PolymorphJsonConverter<T> : JsonConverter<T>
    {
        public override T? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            using var doc = JsonDocument.ParseValue(ref reader);
            JsonElement typeProp = doc.RootElement.GetProperty("$type");
            string typeReference = typeProp.GetString() ?? throw new JsonException("Missing '$type'");
            var actualType = Type.GetType(typeReference, throwOnError: true);

            T inst;
            try
            {
                inst = (T)Activator.CreateInstance(actualType);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException($"Type '{actualType}' not supports constructor with parameters for now", ex);
            }

            JsonConverterHelper.Populate(inst, doc, options);

            return inst;
        }

        public override void Write(
            Utf8JsonWriter writer,
            T value,
            JsonSerializerOptions options)
        {
            if (value is null)
            {
                JsonSerializer.Serialize(writer, value: default, typeof(object));
                return;
            }

            Type valueType = value.GetType();
            var jsonPropInfos = JsonConverterHelper.ResolveJsonPropertyInfos(valueType, options);

            writer.WriteStartObject();
            writer.WriteString("$type", value.GetType().AssemblyQualifiedName);

            foreach (var jsonPropInfo in jsonPropInfos)
            {
                if (jsonPropInfo.Get is null || !jsonPropInfo.Get(value).Let(out object? propValue))
                {
                    writer.WriteNull(jsonPropInfo.Name);
                    continue;
                }

                writer.WritePropertyName(jsonPropInfo.Name);

                JsonSerializer.Serialize(writer, propValue, options);
            }

            writer.WriteEndObject();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsType<T>();
        }
    }
}
