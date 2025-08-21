using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#nullable enable
namespace UTIRLib.Reflection
{
    public readonly struct InvokableSignature :
        IEnumerable<Type>, 
        IEquatable<InvokableSignature>,
        IEquatable<Type[]>
    {
        private readonly Type[] types;

        public static InvokableSignature Empty => new(Type.EmptyTypes);
        public readonly bool AllowInheritance;

        public readonly ReadOnlySpan<Type> Types => new(types);
        public readonly int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => types?.Length ?? 0;
        }
        public readonly Type this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => types[index];
        }

        public InvokableSignature(bool allowInheritance, params Type[] types)
        {
            AllowInheritance = allowInheritance;
            this.types = types;
        }

        public InvokableSignature(params Type[] types)
            :
            this(allowInheritance: false, types)
        {
        }

        public InvokableSignature(IEnumerable<Type> types, bool allowInheritance = false)
            :
            this(allowInheritance, types.ToArray())
        {
        }

        public static InvokableSignature Create(object[] args, bool allowInheritance = false)
        {
            return ProcessArguments(args, allowInheritance);
        }

        public bool Equals(Type[]? other)
        {
            if (other is null)
                return false;

            if (other.IsEmpty() && types.IsEmpty())
                return true;

            if (other.Length != types.Length)
                return false;

            if (AllowInheritance)
            {
                for (int i = 0; i < other.Length; i++)
                {
                    if (types[i].IsNotType(other[i]))
                        return false;
                }
            }
            else
            {
                for (int i = 0; i < other.Length; i++)
                {
                    if (types[i] != other[i])
                        return false;
                }
            }


            return true;
        }
        public bool Equals(InvokableSignature other)
        {
            return Equals(other.types);
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
            var sb = new StringBuilder($"{nameof(InvokableSignature)}: {nameof(AllowInheritance)} = {AllowInheritance}, ");

            if (types.IsEmpty())
                sb.Append("empty");
            else
                sb.AppendJoin(Environment.NewLine, types.Select(x => x.GetName()));

            return sb.ToString();
        }

        public IEnumerator<Type> GetEnumerator() => types.GetEnumeratorT();

        private static void ProcessArray(in Array arr, in List<Type> signature)
        {
            foreach (var item in arr)
            {
                if (item is not null)
                {
                    signature.Add(item.GetType().MakeArrayType());
                    return;
                }
            }

            signature.Add(arr.GetType());
        }

        private static InvokableSignature ProcessArguments(object[] args,
                                                           bool allowTypeInheritance)
        {
            var signature = new List<Type>(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Array arr)
                    ProcessArray(arr, signature);
                else
                    signature.Add(args[i].GetType());
            }

            return new InvokableSignature(signature, allowTypeInheritance);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(InvokableSignature a, Type[] b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(InvokableSignature a, Type[] b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(InvokableSignature a, InvokableSignature b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(InvokableSignature a, InvokableSignature b)
        {
            return !a.Equals(b);
        }

        public static explicit operator Type[](InvokableSignature signatiure)
        {
            return signatiure.Types.ToArray();
        }
    }
}
