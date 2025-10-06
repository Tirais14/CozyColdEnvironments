#nullable enable
namespace CCEnvs.Unity.Extenject
{
    public interface IInjectionWrapping
    {
        object? Value { get; }
    }
    public interface IInjectionWrapping<T> : IInjectionWrapping
    {
        new T Value { get; }

        object? IInjectionWrapping.Value => Value;
    }
}
