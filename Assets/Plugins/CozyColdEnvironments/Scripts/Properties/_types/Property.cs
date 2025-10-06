#nullable enable
namespace CCEnvs.Properties
{
    public class Property<T> : IProperty<T>
    {
        private readonly T defaultValue = default!;
        private T _value = default!;

        public T Value {
            get => _value;
            set
            {
                if (value.IsDefault())
                {
                    _value = defaultValue;
                    return;
                }

                _value = value;
            }
        }

        public Property()
        {
        }

        public Property(T value)
        {
            Value = value;
        }

        public Property(T defaultValue, bool _)
            :
            this(defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        public Property(T value, T defaultValue, bool _)
            :
            this(value)
        {
            this.defaultValue = defaultValue;
        }

        public static implicit operator T(Property<T> source)
        {
            return source.Value;
        }
    }
}
