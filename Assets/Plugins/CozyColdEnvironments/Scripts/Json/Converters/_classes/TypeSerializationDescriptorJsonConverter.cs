using CCEnvs.Json;
using CCEnvs.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable
namespace CCEnvs.Saves
{
    public class TypeSerializationDescriptorJsonConverter : JsonConverter<TypeSerializationDescriptor>
    {
        public override TypeSerializationDescriptor ReadJson(
            JsonReader reader,
            Type objectType,
            TypeSerializationDescriptor existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            var jToken = JToken.Load(reader);

            if (jToken.Type == JTokenType.String)
            {
                var serialized = jToken.ToString();

                if (serialized.IsNullOrWhiteSpace())
                    return default;

                var parts = serialized.Split(", ");

                var name = parts[0];

                var id = parts[1];

                return new TypeSerializationDescriptor(name, id);
            }
            else
            {
                return (TypeSerializationDescriptor)JsonSerializerInternalReaderHelper.CreateNewObject(
                    typeof(TypeSerializationDescriptor),
                    jToken.CreateReader(),
                    serializer
                    );
            }
;        }

        public override void WriteJson(
            JsonWriter writer,
            TypeSerializationDescriptor value,
            JsonSerializer serializer
            )
        {
            if (value.IsDefault())
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue($"{value.Name}, {value.ID}");
        }
    }
}
