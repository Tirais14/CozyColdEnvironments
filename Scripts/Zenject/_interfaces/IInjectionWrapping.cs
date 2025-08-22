#nullable enable
namespace UTIRLib.Zenject
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
