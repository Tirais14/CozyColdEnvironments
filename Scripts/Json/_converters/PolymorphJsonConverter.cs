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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#pragma warning disable S2696
namespace CCEnvs.Json.Converters
{
    public static class PolymorphJsonConverter
    {
        public static Dictionary<Type, Type> DefaultTypeBindings { get; } = new(0);
    }

    public class PolymorphJsonConverter<T> : JsonConverter<T>
    {
        private static int invokesCount;

        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            invokesCount++;

            if (invokesCount > 3)
                throw new JsonSerializationException("Prevented dead loop in polymorph converter.");

            try
            {
                JToken token = ReadToken(ref reader);

                if (JsonSerializerDebug.IsEnabled)
                    CCDebug.PrintLog($"Deserializing token content => {token}", DebugContext.Additive(this));

                Type conversationType = GetConversationType(serializer, token);
                CreateSerializerWithoutThisConverter(conversationType, ref serializer);

                if (JsonSerializerDebug.IsEnabled)
                    CCDebug.PrintLog($"Conversation type = {conversationType.GetFullName()}. Result type = {objectType.GetFullName()}.", DebugContext.Additive(this));

                if (conversationType.IsNotType(objectType))
                    throw new JsonSerializationException($"Invalid conversation type = {conversationType.GetFullName()}.");

                if (!TryGetByCache(conversationType,
                                   out object? intermediate,
                                   out bool isCacheable))
                {
                    intermediate = token.ToObject(conversationType, serializer);

                    if (intermediate.IsNull())
                        throw new JsonSerializationException("Intermediate deserialize returned null.");

                    if (isCacheable)
                        JsonSerializerCache.Values.TryAdd(conversationType, intermediate);
                }

                return CCConvert.ChangeType<T>(intermediate);
            }
            finally
            {
                invokesCount--;
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

        private static void CreateSerializerWithoutThisConverter(Type conversationType, ref JsonSerializer serializer)
        {
            JsonSerializerSettings settings = JsonSettingsProvider.GetSettings();
            var edited = new CCJsonConverterCollection(settings.Converters);

            IEnumerable<Type> conversationTypes = edited.Select(x =>
            {
                try
                {
                    return JsonConverterHelper.GetConversationType(x);
                }
                catch (NotSupportedException)
                {
                    return null!;
                }
            }).Where(x => x.IsNotNull());

            Type elderConversationType = TypeHelper.GetElderType(conversationTypes);

            edited.RemoveAllByType(elderConversationType, out _);

            settings.Converters = edited;

            serializer = JsonSerializer.CreateDefault(settings);
        }

        private static JToken ReadToken(ref JsonReader reader)
        {
            JToken token = JToken.Load(reader);
            reader = token.CreateReader();

            return token;
        }

        private static Type GetConversationType(JsonSerializer serializer, JToken token)
        {

            NamingStrategy namingStrategy = (NamingStrategy)serializer.ContractResolver
                                                .AsReflected()
                                                .Property(nameof(NamingStrategy))
                                                .GetValue();

            string keyName = namingStrategy.GetPropertyName(nameof(ITypeProvider.ObjectType), false);
            JToken? objectTypeToken = token[keyName];

            if (objectTypeToken is null)
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

        private static bool TryGetByCache(Type conversationType,
                                          [NotNullWhen(true)] out object? intermediate,
                                          out bool isCacheable)
        {
            isCacheable = conversationType.IsCacheableType();

            if (isCacheable
                ||
                JsonSerializerCache.Values.ContainsKey(conversationType)
                )
                return JsonSerializerCache.Values.TryGetValue(conversationType,
                                                              out intermediate);

            intermediate = null;
            return false;
        }
    }
}
