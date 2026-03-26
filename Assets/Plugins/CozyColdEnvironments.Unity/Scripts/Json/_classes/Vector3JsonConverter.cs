using CCEnvs.Attributes;
using CCEnvs.Json;
using Newtonsoft.Json;
using System;
using System.Globalization;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Json
{
    public class Vector3JsonConverter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(
            JsonReader reader,
            Type objectType,
            Vector3 existingValue,
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
            Vector3 value,
            JsonSerializer serializer
            )
        {
            writer.WriteValue($"x:{value.x.ToString(serializer.Culture)}; y:{value.y.ToString(serializer.Culture)}; z:{value.z.ToString(serializer.Culture)}");
        }

        [OnInstallExecutable]
        private static void OnInstall()
        {
            CC.SerializerSettings.AddConverters(new Vector3JsonConverter());
        }
    }
}
