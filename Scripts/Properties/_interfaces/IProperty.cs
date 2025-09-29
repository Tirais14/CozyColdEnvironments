#nullable enable
namespace CCEnvs.Properties
{
    public interface IProperty : IReadOnlyProperty
    {
        new object Value { get; set; }

        object IReadOnlyProperty.Value => Value;
    }
    public interface IProperty<T> : IProperty, IReadOnlyProperty<T>
    {
        new T Value { get; set; }

        T IReadOnlyProperty<T>.Value => Value;

        object IProperty.Value {
            get => Value!;
            set => Value = value.As<T>();
        }
        object IReadOnlyProperty.Value => Value!;
    }
}
