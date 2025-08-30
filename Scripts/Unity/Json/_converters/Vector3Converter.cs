using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Json.Converters
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader,
                                         Type objectType,
                                         Vector3 existingValue,
                                         bool hasExistingValue,
                                         JsonSerializer serializer)
        {
            return VectorConverterHelper.ReadVector(reader);
        }

        public override void WriteJson(JsonWriter writer,
                                       Vector3 value,
                                       JsonSerializer serializer)
        {
            VectorConverterHelper.WriteVector(writer, value);
        }
    }
}
