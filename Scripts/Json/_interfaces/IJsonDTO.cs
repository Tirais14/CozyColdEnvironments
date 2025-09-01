#nullable enable
using System;

namespace CCEnvs.Json.DTO
{
    public interface IJsonDto
    {
    }

    public static class IJsonDtoExtensions
    {
        public static object? ConvertTo(this IJsonDto? value, Type toType)
        {
            return DtoConverter.ConvertTo(value, toType);
        }

        public static T? ConvertTo<T>(this IJsonDto? value)
        {
            return DtoConverter.ConvertTo<T>(value);
        }
    }
}
