using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace UTIRLib.Reflection.ObjectModel
{
    public readonly struct ExplicitArguments : IEquatable<ExplicitArguments>
    {
        public readonly Signature signature;
        public readonly Arguments arguments;

        public ExplicitArguments(params TypeValuePair[] pairs)
        {
            signature = new Signature(pairs.Select(x => x.type).ToArray());
            arguments = new Arguments(pairs.Select(x => x.value).ToArray());
        }
        public ExplicitArguments(params object[] args)
            :
            this(args.Select(x => new TypeValuePair(x.GetType(), x)).ToArray())
        {
        }
        public ExplicitArguments(object args, bool singleArg)
            :
            this(args)
        {
        }

        public static bool operator ==(ExplicitArguments left, ExplicitArguments right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ExplicitArguments left, ExplicitArguments right)
        {
            return !left.Equals(right);
        }

        public static explicit operator Type[](ExplicitArguments args)
        {
            return (Type[])args.signature;
        }

        public static explicit operator object?[](ExplicitArguments args)
        {
            return (object?[])args.arguments;
        }

        public static explicit operator TypeValuePair[](ExplicitArguments args)
        {
            return args.ToPairs();
        }

        public TypeValuePair[] ToPairs()
        {
            var typeValuePairs = new List<TypeValuePair>(signature.Count);
            for (int i = 0; i < signature.Count; i++)
                typeValuePairs.Add(new TypeValuePair(signature[i], arguments[i]));

            return typeValuePairs.ToArray();
        }

        public bool Equals(ExplicitArguments other)
        {
            return signature.Equals(other.signature)
                   && 
                   arguments.Equals(other.arguments);
        }
        public override bool Equals(object obj)
        {
            return obj is ExplicitArguments typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(signature, arguments);
        }

        public override string ToString()
        {
            if (signature.IsEmpty())
                return "empty";

            TypeValuePair[] typeValuePairs = ToPairs();
            var builder = new StringBuilder();
            TypeValuePair pair;
            for (int i = 0; i < typeValuePairs.Length; i++)
            {
                pair = typeValuePairs[i];
                builder.Append($"position = {i}, type = {pair.type.GetName()}, value = {pair.value}; ");
            }

            return builder.ToString();
        }
    }
}
