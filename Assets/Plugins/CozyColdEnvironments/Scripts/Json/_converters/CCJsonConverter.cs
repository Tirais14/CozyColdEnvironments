using CCEnvs.Cacheables;
using CCEnvs.Diagnostics;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Linq;
using CCEnvs.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
#pragma warning disable S2743
#pragma warning disable S2696
namespace CCEnvs.Json
{
    public class CCJsonConverter<T> : JsonConverter
    {
        private static int readInokesCount;
        private static int writeInokesCount;

        protected static object? DeserializeObject(Type type,
                                                   JToken token,
                                                   JsonSerializer serializer)
        {
            if (!TryGetCachedObject(type,
                                    out object? deserialized,
                                    out bool isCacheable))
            {
                deserialized = token.ToObject(type, serializer);

                if (deserialized.IsNull())
                    CCDebug.PrintWarning($"Deserialized object ({typeof(T).GetFullName()}) is null.",
                                         typeof(CCJsonConverter<T>));

                if (deserialized is null)
                    return null;

                if (isCacheable)
                    JsonSerializerCache.Objects.TryAdd(type, deserialized);
            }

            return deserialized;
        }

        private static bool TryGetCachedObject(Type type,
                                               [NotNullWhen(true)] out object? result,
                                               out bool isCacheable)
        {
            isCacheable = type.IsCacheableType();

            if (isCacheable
                ||
                JsonSerializerCache.Objects.ContainsKey(type)
                )
                return JsonSerializerCache.Objects.TryGetValue(type,
                                                              out result);

            result = null;
            return false;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsType<T>();
        }

        public override object? ReadJson(JsonReader reader,
                                         Type objectType,
                                         object? existingValue,
                                         JsonSerializer serializer)
        {
            readInokesCount++;

            if (readInokesCount > 3)
                throw new JsonSerializationException($"Prevented dead loop in {GetType().GetFullName()}.");

            try
            {
                PrepareSerializer(ref reader, ref serializer, out JToken token);

                return DeserializeObject(typeof(T), token, serializer);
            }
            finally
            {
                readInokesCount--;
            }
        }

        public override void WriteJson(JsonWriter writer,
                                       object? value,
                                       JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            if (writeInokesCount > 3)
                throw new JsonSerializationException($"Prevented dead loop in {GetType().GetFullName()}.");

            writeInokesCount++;
            try
            {
                serializer.Serialize(writer, value);
            }
            finally
            {
                writeInokesCount--;
            }
        }

        protected void PrepareSerializer(ref JsonReader reader,
                                         ref JsonSerializer serializer,
                                         out JToken token)
        {
            token = ReadToken(ref reader);
            CreateSerializerWithoutThisConverter(ref serializer);
        }

        private void CreateSerializerWithoutThisConverter(ref JsonSerializer serializer)
        {
            JsonSerializerSettings settings = JsonSettingsProvider.GetSettings();
            settings.Converters = settings.Converters.RemoveElement(this).ToArray();

            serializer = JsonSerializer.CreateDefault(settings);
        }

        private JToken ReadToken(ref JsonReader reader)
        {
            JToken token = JToken.Load(reader);

            if (JsonSerializerDebug.IsEnabled)
                CCDebug.PrintLog($"Deserializing token content => {token}",
                                 new DebugContext(this).Additive());

            reader = token.CreateReader();

            return token;
        }
    }
}
