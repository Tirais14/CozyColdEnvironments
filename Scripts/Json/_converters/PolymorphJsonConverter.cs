#nullable enable
using CCEnvs.Cacheables;
using CCEnvs.Diagnostics;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

#pragma warning disable IDE0044
#pragma warning disable S3963
namespace CCEnvs.Json.Converters
{
    public static class PolymorphJsonConverter
    {
        public static Dictionary<Type, Type> DefaultTypeBindings { get; } = new(0);
    }

    public class PolymorphJsonConverter<T> : JsonConverter<T>
    {
        private static JsonSerializerSettings? setting;

        static PolymorphJsonConverter()
        {
            //To avoid dead loop
            JsonSerializerSettings settings = JsonSettingsProvider.GetSettings();
            var temp = new CCJsonConverterCollection(settings.Converters);
            temp.RemoveAllByType(typeof(T), out _);

            settings.Converters = temp;

            setting = settings;
        }

        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            JToken token = ReadToken();

            if (JsonSerializerDebug.IsEnabled)
                CCDebug.PrintLog($"Deserializing token content => {token}", this);

            Type conversationType = GetConversationType();
            object? intermediate = null!;
            if (!TryGetByCache(out bool isCacheable))
            {
                intermediate = token.ToObject(conversationType, JsonSerializer.Create(setting));

                if (intermediate.IsNull())
                    throw new JsonSerializationException("Intermediate deserialize returned null.");

                if (isCacheable)
                    JsonSerializerCache.TryCache(conversationType, intermediate);
            }
                
            return CCConvert.ChangeType<T>(intermediate);

            Type GetConversationType()
            {
                NamingStrategy namingStrategy = (NamingStrategy)serializer.ContractResolver
                                                    .AsReflected()
                                                    .PropertyGet(nameof(NamingStrategy), nonPublic: true);

                string keyName = namingStrategy.GetPropertyName(nameof(ITypeProvider.ObjectType), false);
                JToken? objectTypeToken = token[keyName];

                if (objectTypeToken.IsNull())
                {
                    if (PolymorphJsonConverter.DefaultTypeBindings.TryGetValue(
                    typeof(T),
                    out Type result)
                    )
                        return result;

                    throw new JsonSerializationException($"The converter needs at least one binding: {PolymorphJsonConverter.DefaultTypeBindings} or objectType key in json data to resolve conversation type.");
                }


                return objectTypeToken.ToObject<Type>(serializer)
                       ??
                       CC.Throw.InvalidCast(typeof(JToken)).As<Type>();
            }

            bool TryGetByCache(out bool isCacheable)
            {
                isCacheable = conversationType.IsCacheableType();

                if (isCacheable || JsonSerializerCache.IsBinded(conversationType))
                    return JsonSerializerCache.TryGetCached(conversationType, out intermediate);

                return false;
            }

            JToken ReadToken()
            {
                JToken token = JToken.Load(reader);
                reader = token.CreateReader();

                return token;
            }
        }

        public override void WriteJson(JsonWriter writer,
                                       T? value,
                                       JsonSerializer serializer)
        {
            if (value.IsNull())
            {
                writer.WriteNull();
                return;
            }

            var dto = InstanceFactory.Create(((ITypeProvider)value).ObjectType,
                new ExplicitArguments(new ExplicitArgument(value)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound
                |
                InstanceFactory.Parameters.NonPublic);

            serializer.Serialize(writer, dto);
        }
    }
}
