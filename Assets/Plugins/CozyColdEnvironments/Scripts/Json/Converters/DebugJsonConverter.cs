using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class DebugJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            this.PrintLog($"Reading type \"{objectType}\"");

            var token = JToken.ReadFrom(reader);

            foreach (var childToken in token)
                this.PrintLog($"Readed \"{childToken}\"");

            return !objectType.IsClass ? Activator.CreateInstance(objectType) : null;
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer)
        {
            this.PrintLog($"Writing type \"{value}\"");

            var toWrite = JsonConvert.SerializeObject(value);

            this.PrintLog($"To write \"{toWrite}\"");
        }
    }
}
