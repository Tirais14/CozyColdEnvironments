#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CCEnvs.Json.Converters
{
    public class PolymorphJsonConverter<TIntermediate, T> : JsonConverter<T>
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            if (typeof(TIntermediate) == typeof(T))
                throw new JsonSerializationException($"{nameof(TIntermediate)} cannot equals {nameof(T)}.");

            if (JsonSerializerDebug.IsEnabled)
            {
                var token = JToken.Load(reader);
                CCDebug.PrintLog($"{this.GetTypeName()}: deserializing token.{Environment.NewLine}{token}");
                reader = token.CreateReader();
            }

            //To avoid dead loop
            serializer = JsonSerializer.Create(JsonSettingsProvider.GetSettings());
            serializer.Converters.Remove(this);

            TIntermediate? intermediate;
            Type intermediateType = typeof(TIntermediate);
            if (JsonDtoCache.IsBinded(intermediateType)
                ||
                intermediateType.IsCacheableType())
            {
                if (!JsonDtoCache.TryGetCached(intermediateType, out intermediate))
                {
                    intermediate = serializer.Deserialize<TIntermediate>(reader);

                    if (intermediate.IsDefault())
                        throw new DeserializeDataException(intermediateType);

                    JsonDtoCache.TryCache(intermediateType, intermediate);
                }
            }
            else
                intermediate = serializer.Deserialize<TIntermediate>(reader);

            if (intermediate.IsDefault())
                return default;

            Type resultType = typeof(T);
            if (resultType.IsInterface
               ||
               resultType.IsAbstract
               &&
               resultType.IsNotType<IConvertibleCC>()
               )
                throw new JsonSerializationException($"Cannot convert {resultType} because it is abstract or interface type. For this conversations object must implement {nameof(IConvertibleCC)}");

            return CCConvert.ChangeType<T>(intermediate);
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

            var dto = InstanceFactory.Create<TIntermediate>(
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
