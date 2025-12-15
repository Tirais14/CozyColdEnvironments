using CCEnvs.Diagnostics;
using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CCEnvs.Snapshots;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable
namespace CCEnvs.Json.Converters
{
    public class SnapshotConverter : JsonConverter<ISnapshot>
    {
        public override ISnapshot? ReadJson(
            JsonReader reader,
            Type objectType,
            ISnapshot? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (hasExistingValue && existingValue is not null)
            {
                serializer.Populate(reader, existingValue);
                return existingValue;
            }

            var jObject = JObject.Load(reader);

            if (!jObject.TryGetValue("selfType", StringComparison.OrdinalIgnoreCase, out var typeToken))
                throw new JsonSerializationException("Missing 'selfType' property");

            var typeSnapshot = typeToken.ToObject<TypeSnapshot>().Maybe().GetValue(() => throw new JsonSerializationException($"Cannot deserialize: selfType"));

            Type actualType = typeSnapshot.Restore();

            if (!actualType.IsNotType<ISnapshot>())
                throw new JsonSerializationException($"Type '{actualType}' does not implement {nameof(ISnapshot)}");

            using var newReader = jObject.CreateReader();
            return (ISnapshot?)serializer.Deserialize(newReader, actualType);
        }

        public override void WriteJson(
            JsonWriter writer,
            ISnapshot? value,
            JsonSerializer serializer)
        {
            if (value.IsNull())
                writer.WriteNull();

            serializer.Serialize(writer, value);
        }
    }
}
