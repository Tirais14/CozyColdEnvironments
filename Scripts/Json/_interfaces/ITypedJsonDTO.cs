#nullable enable
using System;

#pragma warning disable S101
namespace CCEnvs.Json.DTO
{
    public interface ITypedJsonDto : IJsonDto
    {
        public Type ObjectType { get; }
    }

    public static class ITypedJsonDTOExtensions
    {
        public static object? Value(this ITypedJsonDto? value)
        {
            return DtoConverter.Convert(value);
        }

        public static T? Value<T>(this ITypedJsonDto? value)
        {
            return DtoConverter.Convert<T>(value);
        }
    }
}
