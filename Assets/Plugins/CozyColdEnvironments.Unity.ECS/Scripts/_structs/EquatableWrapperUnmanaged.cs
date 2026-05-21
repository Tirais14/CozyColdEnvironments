using System;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public readonly struct EquatableWrapperUnmanaged<T> : IEquatable<EquatableWrapperUnmanaged<T>>, IEquatable<T>
        where T : unmanaged
    {
        public readonly T Value;

        public EquatableWrapperUnmanaged(T value)
        {
            Value = value;
        }

        public readonly bool Equals(T other)
        {
            return Value switch
            {
                IEquatable<T> eqt => eqt.Equals(other),
                _ => Value.Equals(other)
            };
        }

        public readonly bool Equals(EquatableWrapperUnmanaged<T> other) => Equals(other.Value);

        public readonly override bool Equals(object obj)
        {
            return obj switch
            {
                EquatableWrapperUnmanaged<T> wrapper => Equals(wrapper),
                T value => Equals(value),
                _ => false,
            };
        }

        public readonly override int GetHashCode() => Value.GetHashCode();

        public readonly override string ToString() => Value.ToString();
    }
}
