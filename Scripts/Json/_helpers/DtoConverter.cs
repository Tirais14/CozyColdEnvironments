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
        public static object? Convert(object? dto, Type toType)
        {
            if (dto.IsNull())
                return null;
            if (toType is null)
                throw new ArgumentNullException(nameof(toType));

            if (JsonSettingsProvider.TryGetDtoConverter(dto.GetType(),
                                                        out CCJsonConverterFunc? method))
                return method(dto);

            if (dto is ICCConvertible convertible
                &&
                convertible.Convert() is object temp
                &&
                temp.GetType().IsType(toType)
                )
                return temp;

            if (toType.IsInterface || toType.IsAbstract)
                return default;

            object? result = InstanceFactory.Create(toType,
                new ExplicitArguments(new ExplicitArgument(dto)),
                InstanceFactory.Parameters.CacheConstructor);

            //if (result.IsNotNull())
            //    return result;

            //result = InstanceFactory.CreateBy(
            //    toType,
            //    dto,
            //    InstanceFactory.Parameters.CacheConstructor);

            if (result.IsNull())
                throw new InstanceCreationException(toType);

            return result;
        }
        public static T? Convert<T>(object? dto)
        {
            return (T?)Convert(dto, typeof(T));
        }

        /// <exception cref="ArgumentException"></exception>
        public static object? Convert(ITypedJsonDto? dto)
        {
            if (dto.IsDefault())
                return default;
            if (dto.ObjectType is null)
                throw new ArgumentException(nameof(dto.ObjectType));

            return Convert(dto, dto.ObjectType);
        }
        /// <exception cref="ArgumentException"></exception>
        public static T? Convert<T>(ITypedJsonDto? dto)
        {
            if (dto.IsDefault())
                return default;
            if (dto.ObjectType is null)
                throw new ArgumentException(nameof(dto.ObjectType));
            if (dto.ObjectType.IsNotType(typeof(T)))
                throw new ArgumentException(nameof(dto.ObjectType));

            return (T?)Convert(dto, dto.ObjectType);
        }
    }
}
