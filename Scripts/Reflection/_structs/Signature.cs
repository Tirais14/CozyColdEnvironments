using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#nullable enable
namespace CCEnvs.Reflection.ObjectModel
{
    public readonly struct Signature
        :
        IReadOnlyList<Type>,
        IEquatable<Signature>
    {
        private readonly ReadOnlyCollection<Type> types;

        public IReadOnlyList<Type> Types => types;
        public int Count => types?.Count ?? 0;
        public Type this[int index] => types[index];

        public Signature(params Type[] types)
        {
            this.types = new ReadOnlyCollection<Type>(types);
        }
        public Signature(IEnumerable<Type> types)
            :
            this(types.ToArray())
        {
        }

        public static bool operator ==(Signature left, Signature right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Signature left, Signature right)
        {
            return !left.Equals(right);
        }

        public static explicit operator Type[](Signature signature)
        {
            return signature.types?.ToArray() ?? Type.EmptyTypes;
        }

        public bool Equals(Signature other)
        {
            if (types is null && other.types is null)
                return true;
            if (other.types is null)
                return false;

            return types.SequenceEqual(other.types);
        }
        public override bool Equals(object obj)
        {
            return obj is Signature typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            for (int i = 0; i < types.Count; i++)
                hash.Add(types[i]);

            return hash.ToHashCode();
        }

        public override string ToString()
        {
            if (((ICollection<Type>)types).IsNullOrEmpty())
                return "empty";

            var builder = new StringBuilder();
            for (int i = 0; i < types.Count; i++)
                builder.Append($"Position = {i}, type = {types[i].GetName()}; ");

            return builder.ToString();
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return types?.GetEnumerator() ?? TEnumerable<Type>.Empty.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
