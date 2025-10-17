#nullable enable
namespace CCEnvs.Properties
{
    public class ResetableProperty<T>
    {
        private readonly T? defaultValue;
        private T? value;

        public ResetableProperty(T? defaultValue, T? value)
        {
            this.defaultValue = defaultValue;
            this.value = value;
        }

        public ResetableProperty(T? value)
            :
            this(defaultValue: default, value)
        {
        }

        public ResetableProperty()
            :
            this(defaultValue: default, value: default)
        {
        }

        public static explicit operator T?(ResetableProperty<T> source)
        {
            return source.value;
        }

        /// <summary>
        /// returns <see cref="value"/> and reset it in property
        /// </summary>
        /// <returns></returns>
        public T? Use()
        {
            T? temp = value;
            Reset();

            return temp;
        }

        public void Reset()
        {
            value = defaultValue;
        }
    }
}
