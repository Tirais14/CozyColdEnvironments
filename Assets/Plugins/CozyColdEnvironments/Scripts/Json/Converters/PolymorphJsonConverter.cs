using CCEnvs.Reflection;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public static class PolymorphJsonConverter
    {
        internal static MemoryCache Cache { get; } = new(
            new MemoryCacheOptions
            {
                ExpirationScanFrequency = 1.Minutes(),
            });
    }

    public class PolymorphJsonConverter<T> : JsonConverter<T>
    {
        public override T? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var doc = JsonDocument.ParseValue(ref reader);
            JsonElement typeProp = doc.RootElement.GetProperty("$type");
            string typeReference = typeProp.GetString() ?? throw new JsonException("Missing '$type'");
            var actualType = Type.GetType(typeReference, throwOnError: true);

            JsonSerializerOptions configuredOptions = GetConfiguredOptions(options);
            return (T?)JsonSerializer.Deserialize(doc, actualType, configuredOptions);
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

            var toSerilize = JsonConverterHelper.PrepareToSerilize(value, options);

            writer.WriteStartObject();
            writer.WriteString("$type", value.GetType().AssemblyQualifiedName);
;
            foreach (var (propName, propValue) in toSerilize)
            {
                if (propValue is null)
                {
                    writer.WriteNull(propName);
                    continue;
                }

                writer.WritePropertyName(propName);
                JsonSerializer.Serialize(writer, propValue, propValue.GetType(), options);
            }

            writer.WriteEndObject();
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsType<T>();
        }

        private JsonSerializerOptions GetConfiguredOptions(JsonSerializerOptions options)
        {
            return PolymorphJsonConverter.Cache.GetOrCreate((GetType(), GetType().GetGenericArguments()),
                (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = 5.Minutes();

                    options = new JsonSerializerOptions(options);

                    Type thisType = GetType();
                    foreach (var conv in options.Converters.ToArray().Where(conv => conv.GetType() == thisType))
                        options.Converters.Remove(conv);

                    return options;
                }) 
                ??
                throw new InvalidOperationException($"Missing '{nameof(JsonSerializerOptions)}'");
        }
    }
}
