using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace UTIRLib.Reflection
{
    public readonly struct InvokableSignature : IEnumerable<Type>, IEquatable<InvokableSignature>
    {
        private readonly Type[] types;

        public readonly ReadOnlySpan<Type> Types => new(types);
        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => types.Length;
        }
        public readonly Type this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => types[index];
        }

        public InvokableSignature(params Type[] types)
        {
            this.types = types;
        }

        public InvokableSignature(IEnumerable<Type> types)
            :
            this(types.ToArray())
        {
        }

        public bool Equals(InvokableSignature other)
        {
            if (other.types.Length != types.Length)
                return false;

            Type[] otherTypes = other.types;
            for (int i = 0; i < Types.Length; i++)
            {
                if (types[i] != otherTypes[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is InvokableSignature typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            var code = new HashCode();

            for (int i = 0; i < types.Length; i++)
                code.Add(types[i]);

            return code.ToHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{nameof(InvokableSignature)}: ");

            if (types.IsEmpty())
                sb.Append("empty");
            else
                sb.AppendJoin(", ", types.Select(x => x.GetName()));

            return sb.ToString();
        }

        public IEnumerator<Type> GetEnumerator() => types.GetEnumeratorT();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(InvokableSignature a, InvokableSignature b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(InvokableSignature a, InvokableSignature b)
        {
            return !a.Equals(b);
        }
    }
}
