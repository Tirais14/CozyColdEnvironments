#nullable enable
namespace CCEnvs.Json.DTO
{
    public interface IJsonDtoConvertible
    {
        object ConvertToValue();
    }
    public interface IJsonDtoConvertible<out T> : IJsonDtoConvertible
    {
        new T ConvertToValue();

        object IJsonDtoConvertible.ConvertToValue() => ConvertToValue()!;
    }
}
