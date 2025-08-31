#nullable enable
#pragma warning disable S101
namespace CCEnvs.Json.DTO
{
    public interface IJsonDto
    {
        object ConvertToValue();
    }
    public interface IJsonDto<out T> : IJsonDto
    {
        new T ConvertToValue();

        object IJsonDto.ConvertToValue() => ConvertToValue()!;
    }
}
