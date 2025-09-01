#nullable enable
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Reflection.ObjectModel;
using Newtonsoft.Json;
using System;

namespace CCEnvs.Json
{
    public class TypedJsonConverter<Tdto, T>
        :
        JsonConverter<T>
        where Tdto : ITypedJsonDTO
    {
        public override T? ReadJson(JsonReader reader,
                                    Type objectType,
                                    T? existingValue,
                                    bool hasExistingValue,
                                    JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<Tdto>(reader);

            if (dto is IJsonDtoConvertible<T> convertible)
                return convertible.ConvertToValue();

            return DtoConverter.Convert<T>(dto);
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

            InstanceFactory.Create(typeof(Tdto),
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(value)
                },
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);

            serializer.Serialize(writer, value);
        }
    }
}
