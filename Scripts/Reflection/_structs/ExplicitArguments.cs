using System;
using System.Linq;
using UnityEngine;

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

        //TODO: ToString Method
    }
}
