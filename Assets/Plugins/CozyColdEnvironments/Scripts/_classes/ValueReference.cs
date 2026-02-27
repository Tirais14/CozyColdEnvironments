#nullable enable
namespace CCEnvs
{
    public static class ValueReference
    {
        public static ValueReference<T> From<T>(T value)
        {
            return new ValueReference<T>(value);
        }
    }

    public sealed class ValueReference<T>
    {
        public T Value { get; set; } = default!;

        public ValueReference()
        {
        }

        public ValueReference(T value)
        {
            Value = value;
        }

        public static implicit operator ValueReference<T>(T value)
        {
            return new ValueReference<T>(value);
        }

        public static implicit operator T(ValueReference<T> instance)
        {
            return instance.Value;
        }

    }
}
