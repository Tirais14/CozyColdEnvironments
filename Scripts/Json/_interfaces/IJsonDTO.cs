#nullable enable
using CCEnvs.Conversations;
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
            return TypeTransformer.DoTransform(value, toType);
        }

        public static T? ConvertTo<T>(this IJsonDto? value)
        {
            return TypeTransformer.DoTransform<T>(value);
        }
    }
}
