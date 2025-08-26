using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace UTIRLib.Json.Converters
{
    public class Vector3IntConverter : JsonConverter<Vector3Int>
    {
        public override Vector3Int ReadJson(JsonReader reader,
                                            Type objectType,
                                            Vector3Int existingValue,
                                            bool hasExistingValue,
                                            JsonSerializer serializer)
        {
            var vector = VectorConverterHelper.ReadVector(reader);

            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }

        public override void WriteJson(JsonWriter writer,
                                       Vector3Int value,
                                       JsonSerializer serializer)
        {
            VectorConverterHelper.WriteVector(writer, value);
        }
    }
}
