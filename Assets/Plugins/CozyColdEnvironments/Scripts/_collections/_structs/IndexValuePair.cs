using System;

#nullable enable
namespace CCEnvs
{
    public readonly struct IndexValuePair<T> : IEquatable<IndexValuePair<T>>
    {
        public readonly int index;
        public readonly T value;

        public IndexValuePair(int index, T value)
        {
            this.index = index;
            this.value = value;
        }

        public static bool operator ==(IndexValuePair<T> left, IndexValuePair<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IndexValuePair<T> left, IndexValuePair<T> right)
        {
            return !(left == right);
        }

        public bool Equals(IndexValuePair<T> other)
        {
            return index.Equals(other.index) && value.Equals(other.value);
        }

        public override bool Equals(object obj)
        {
            return obj is IndexValuePair<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(index, value);
        }
    }
}
