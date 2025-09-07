#nullable enable
using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Json.Diagnsotics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CCEnvs.Json.Converters
{
    public class PolymorphTypedJsonConverter<TDto, T> : JsonConverter<T>
        where TDto : ITypeProvider
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            if (JsonSerializerDebug.IsEnabled)
            {
                var token = JToken.Load(reader);

                CCDebug.PrintLog($"{this.GetTypeName()}: deserializing token.{Environment.NewLine}{token}");

                reader = token.CreateReader();
            }

            TDto? dto;
            Type dtoType = typeof(TDto);
            if ((JsonDtoCache.IsBinded(dtoType)
                ||
                dtoType.IsCacheableType()))
            {
                if (!JsonDtoCache.TryGetCached(dtoType, out dto))
                {
                    dto = serializer.Deserialize<TDto>(reader);

                    if (dto.IsDefault())
                        throw new DeserializeDataException(typeof(TDto));

                    JsonDtoCache.TryCache(dtoType, dto);
                }
            }
            else
                dto = serializer.Deserialize<TDto>(reader);

            return CCConvert.Convert<T>(dto);
        }

        public override void WriteJson(JsonWriter writer,
                                       T? value,
                                       JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            InstanceFactory.Create(typeof(TDto),
                new ExplicitArguments(new ExplicitArgument(value)),
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);

            serializer.Serialize(writer, value);
        }
    }
}
