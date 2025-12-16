using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class DebugJsonConverter : JsonConverter<object>
    {
        public override object? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            this.PrintLog($"Reading type '{typeToConvert}'");

            var doc = JsonDocument.ParseValue(ref reader);

            foreach (var prop in doc.RootElement.EnumerateObject())
                this.PrintLog($"Readed '{prop}'");

            return null;
        }

        public override void Write(
            Utf8JsonWriter writer,
            object value,
            JsonSerializerOptions options)
        {
            this.PrintLog($"Writing type '{value}'");

            var toWrite = JsonSerializer.Serialize(value, options.ExcludeConverters(this));

            this.PrintLog($"To write '{toWrite}'");
        }

        public override bool CanConvert(Type typeToConvert) => true;
    }
}
