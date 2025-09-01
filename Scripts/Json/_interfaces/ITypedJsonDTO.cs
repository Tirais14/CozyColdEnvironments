#nullable enable
using System;

#pragma warning disable S101
namespace CCEnvs.Json.DTO
{
    public interface ITypedJsonDTO : IJsonDto
    {
        public Type ObjectType { get; }
    }

    public static class ITypedJsonDTOExtensions
    {
        public static object? Value(this ITypedJsonDTO? value)
        {
            return DtoConverter.Convert(value);
        }

        public static T? Value<T>(this ITypedJsonDTO? value)
        {
            return DtoConverter.Convert<T>(value);
        }
    }
}
