#nullable enable
using CCEnvs.Diagnostics;
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

            if (dto.IsDefault())
                return default;

            if (dto.ObjectType is null)
                throw new DataAccessException(dto.ObjectType);

            if (dto.ObjectType.IsNotType(typeof(T)))
                throw new TypeIsNotExpectedTypeException(dto.ObjectType, typeof(T));

            return InstanceFactory.Create<T>(
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(dto)
                },
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.ThrowIfNotFound);
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
