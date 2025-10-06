#nullable enable
namespace CCEnvs.Properties
{
    public interface IReadOnlyProperty
    {
        object Value { get; }
    }
    public interface IReadOnlyProperty<out T> : IReadOnlyProperty
    {
        new T Value { get; }

        object IReadOnlyProperty.Value => Value!;
    }
}
