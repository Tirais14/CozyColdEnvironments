using UnityEngine;

#nullable enable
namespace UTIRLib.Properties
{
    public class ResetableProperty<T>
    {
        private readonly T? defaultValue;

        public T? Value { get; set; }

        public ResetableProperty(T? defaultValue, T? value)
        {
            this.defaultValue = defaultValue;
            Value = value;
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

        public static implicit operator T?(ResetableProperty<T> prop)
        {
            return prop.Value;
        }

        public T? Use()
        {
            T? temp = Value;
            Reset();

            return temp;
        }

        public void Reset()
        {
            Value = defaultValue;
        }
    }
}
