using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CozyColdEnvironments.Json.Converters
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override Quaternion ReadJson(JsonReader reader,
                                            Type objectType,
                                            Quaternion existingValue,
                                            bool hasExistingValue,
                                            JsonSerializer serializer)
        {
            var angles = serializer.Deserialize<Vector3>(reader);

            return Quaternion.Euler(angles);
        }

        public override void WriteJson(JsonWriter writer,
                                       Quaternion value,
                                       JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.eulerAngles);
        }
    }
}
