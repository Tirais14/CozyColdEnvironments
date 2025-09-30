#nullable enable
using System;

namespace CCEnvs.Properties
{
    /// <summary>
    /// Have similar functionality as <see cref="Lazy{T}"/>, but also have ability to override default value and recreate main value if it's default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyProperty<T> : ILazyProperty<T>
    {
        private readonly T defaultValue = default!;
        private readonly Func<T>? valueFactory;
        private T? _value;

        public T Value {
            get
            {
                if ((!ValueCreated
                    ||
                    !HasValue
                    &&
                    RecreateValueIfDefault)
                    &&
                    valueFactory is not null)
                {
                    _value = valueFactory();
                    ValueCreated = true;
                }

                return _value!;
            }
        }
        public bool RecreateValueIfDefault { get; }
        public bool ValueCreated { get; private set; }
        public bool HasValue => _value.IsNotDefault(Range.From((object)defaultValue!));

        public LazyProperty(T value)
        {
            _value = value;
        }

        public LazyProperty(T value, T defaultValue)
            :
            this(value)
        {
            this.defaultValue = defaultValue;
        }

        public LazyProperty(Func<T> valueFactory)
        {
            this.valueFactory = valueFactory;
        }

        public LazyProperty(Func<T> valueFactory, T defaultValue)
            :
            this(valueFactory)
        {
            this.defaultValue = defaultValue;
        }

        public LazyProperty(Func<T> valueFactory, bool recreateValueIfDefault)
            :
            this(valueFactory)
        {
            RecreateValueIfDefault = recreateValueIfDefault;
        }

        public LazyProperty(Func<T> valueFactory,
                            T defaultValue,
                            bool recreateValueIfDefault)
            :
            this(valueFactory, defaultValue)
        {
            RecreateValueIfDefault = recreateValueIfDefault;
        }

        public static implicit operator T(LazyProperty<T> source)
        {
            return source.Value;
        }
    }
}
