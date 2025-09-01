#nullable enable
namespace CCEnvs.Json.DTO
{
    /// <summary>
    /// Only for custom converts, do not use <see cref="DtoConverter"/> in this
    /// </summary>
    public interface IJsonDtoConvertible
    {
        object ConvertToValue();
    }
    /// <summary>
    /// Only for custom converts, do not use <see cref="DtoConverter"/> in this
    /// </summary>
    public interface IJsonDtoConvertible<out T> : IJsonDtoConvertible
    {
        new T ConvertToValue();

        object IJsonDtoConvertible.ConvertToValue() => ConvertToValue()!;
    }
}
