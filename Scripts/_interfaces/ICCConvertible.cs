#nullable enable
namespace CCEnvs
{
    public interface ICCConvertible
    {
        object Convert();
    }
    public interface ICCConvertible<out T> : ICCConvertible
    {
        new T Convert();

        object ICCConvertible.Convert() => Convert()!;
    }
}
