#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.ObjectModel;
using System;

namespace CCEnvs.Json.DTO
{
    public static class DtoConverter
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DataAccessException"></exception>
        /// <exception cref="TypeIsNotExpectedTypeException"></exception>
        public static object? Convert(ITypedJsonDTO? dto, Type? refType = null)
        {
            if (dto.IsNull())
                return null;
            if (dto.ObjectType is null)
                throw new DataAccessException(dto.ObjectType);
            if (refType is not null && dto.ObjectType.IsNotType(refType))
                throw new TypeIsNotExpectedTypeException(dto.ObjectType, refType);

            if (JsonSettingsProvider.TryGetDtoConverter(dto.GetType(), out Func<IJsonDto, object>? method))
                return method(dto);

            object? result = InstanceFactory.Create(dto.ObjectType,
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(dto)
                },
                InstanceFactory.Parameters.CacheConstructor);

            if (result.IsNull())
                result = InstanceFactory.CreateBy(
                    dto.ObjectType,
                    dto,
                    InstanceFactory.Parameters.CacheConstructor);

            return result;
        }
        public static T? Convert<T>(ITypedJsonDTO? dto)
        {
            return (T?)Convert(dto, typeof(T));
        }
    }
}
