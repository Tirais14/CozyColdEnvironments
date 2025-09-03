#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using System;

namespace CCEnvs.Json.DTO
{
    public static class DtoConverter
    {
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InstanceCreationException"></exception>
        public static object? ConvertTo(IJsonDto? dto, Type toType)
        {
            if (dto.IsNull())
                return null;
            if (toType is null)
                throw new ArgumentNullException(nameof(toType));

            if (JsonSettingsProvider.TryGetDtoConverter(dto.GetType(),
                                                        out Func<IJsonDto, object>? method))
                return method(dto);

            if (dto is IJsonDtoConvertible convertible
                &&
                convertible.ConvertToValue() is object temp
                &&
                temp.GetType().IsType(toType)
                )
                return temp;

            if (toType.IsInterface || toType.IsAbstract)
                return default;

            object? result = InstanceFactory.Create(toType,
                new ConstructorBindings
                {
                    BindingFlags = BindingFlagsDefault.InstanceAll,
                    Arguments = new ExplicitArguments(dto)
                },
                InstanceFactory.Parameters.CacheConstructor);

            if (result.IsNotNull())
                return result;

            result = InstanceFactory.CreateBy(
                toType,
                dto,
                InstanceFactory.Parameters.CacheConstructor);

            if (result.IsNotNull())
                return result;

            throw new InstanceCreationException(toType);
        }
        public static T? ConvertTo<T>(IJsonDto? dto)
        {
            return (T?)ConvertTo(dto, typeof(T));
        }

        public static object? Convert(ITypedJsonDTO? dto)
        {
            if (dto.IsDefault())
                return default;
            if (dto.ObjectType is null)
                throw new DataAccessException(dto.ObjectType);

            return ConvertTo(dto, dto.ObjectType);
        }
        public static T? Convert<T>(ITypedJsonDTO? dto)
        {
            if (dto.IsDefault())
                return default;
            if (dto.ObjectType is null)
                throw new DataAccessException(dto.ObjectType);
            if (dto.ObjectType.IsNotType(typeof(T)))
                throw new ArgumentException(nameof(dto.ObjectType));

            return (T?)ConvertTo(dto, dto.ObjectType);
        }
    }
}
