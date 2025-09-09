#nullable enable
using CCEnvs.Attributes;
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

#pragma warning disable 
namespace CCEnvs.Json.Converters
{
    public static class PolymorphJsonConverter
    {
        private static Dictionary<Type, Type> bindings = new(0);

        public static void Bind(Type contractType,
                                Type deserializedType,
                                bool trimCollectionCapacity = false)
        {
            CC.Validate.ArgumentNull(contractType, nameof(contractType));
            CC.Validate.ArgumentNull(deserializedType, nameof(deserializedType));

            if (HasBinding(contractType))
                throw new ArgumentException($"{contractType.GetFullName()} already binded");

            bindings.Add(contractType, deserializedType);
        }

        public static bool RemoveBinding(Type? contractType)
        {
            if (contractType is null)
                return false;

            return bindings.Remove(contractType);
        }

        public static bool HasBinding(Type? contractType)
        {
            if (contractType is null)
                return false;

            return bindings.ContainsKey(contractType);
        }
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
            if (!TryGetByCache(conversationType, out intermediate, out bool isCacheable))
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
                JToken objectTypeToken = token[keyName]
                       ??
                       throw new JsonSerializationException($"Cannot find {keyName} key in json data. Polymorph converter doesn't work without this key."); ;

                return objectTypeToken.ToObject<Type>(serializer);
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

        private static bool TryGetByCache(Type conversationType,
                                          [NotNullWhen(true)] out object? intermediate,
                                          out bool isCacheable)
        {
            isCacheable = conversationType.IsCacheableType();

            if (isCacheable || JsonSerializerCache.IsBinded(conversationType))
            {
                MethodInfo[] cacheAccessors =
                    (from method in conversationType.ForceGetMethods(BindingFlagsDefault.All)
                     where method.IsDefined<CacheAccessorAttribute>(inherit: true)
                     select method).
                     ToArray();

                if (cacheAccessors)

                return JsonSerializerCache.TryGetCached(conversationType, out intermediate);
            }

            intermediate = null;
            return false;
        }
    }
}
