using CCEnvs.Attributes;
using CCEnvs.Json;
using Newtonsoft.Json;
using System;
using System.Globalization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public class Vector4JsonConverter : JsonConverter<Vector4>
    {
        public override Vector4 ReadJson(
            JsonReader reader,
            Type objectType,
            Vector4 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
            )
        {
            if (reader.TokenType == JsonToken.Null
                ||
                reader.Value is not string serialized)
            {
                return default;
            }

            var raw = serialized.Split("; ");

            for (int i = 0; i < raw.Length; i++)
                existingValue[i] = float.Parse(raw[i].Split(':')[1], serializer.Culture);

            return existingValue;
        }

        public override void WriteJson(
            JsonWriter writer,
            Vector4 value,
            JsonSerializer serializer
            )
        {
            writer.WriteValue($"x:{value.x.ToString(serializer.Culture)}; y:{value.y.ToString(serializer.Culture)}; z:{value.z.ToString(serializer.Culture)}; w:{value.w.ToString(serializer.Culture)}");
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            CC.SerializerSettings.AddConverters(new Vector4JsonConverter());
        }
    }
}
