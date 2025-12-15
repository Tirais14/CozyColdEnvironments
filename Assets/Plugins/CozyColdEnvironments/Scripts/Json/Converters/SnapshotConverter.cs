using CCEnvs.Snapshots;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class SnapshotConverter : JsonConverter<Snapshot>
    {
        public override Snapshot? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var doc = JsonDocument.ParseValue(ref reader);
            var selfTypeProperty = doc.RootElement.GetProperty("selfType");
            var selfTypeContent = selfTypeProperty.GetString() ?? throw new JsonException("Missing 'selfTypeContent'");
            var configuredOptions = GetConfiguredOptions(options);
            var actualTypeSnapshot = JsonSerializer.Deserialize<TypeSnapshot>(selfTypeContent, configuredOptions);
            Type actualType = actualTypeSnapshot.Restore();

            if (actualType is null)
                throw new JsonException($"Missing '{nameof(actualType)}'");

            return (Snapshot?)JsonSerializer.Deserialize(doc, actualType, configuredOptions);
        }

        public override void Write(
            Utf8JsonWriter writer,
            Snapshot value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(value, GetConfiguredOptions(options));
        }

        private static JsonSerializerOptions GetConfiguredOptions(JsonSerializerOptions options)
        {
            options = new JsonSerializerOptions(options);

            foreach (var conv in options.Converters.Where(conv => conv is JsonConverter<Snapshot> || conv.CanConvert(typeof(TypeSnapshot))))
                options.Converters.Remove(conv);

            return options;
        }
    }
}
