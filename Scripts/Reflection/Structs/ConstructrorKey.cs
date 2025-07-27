using System;
using UnityEngine.VFX;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib
{
    public readonly struct ConstructrorKey : IEquatable<ConstructrorKey>
    {
        public readonly Type Type { get; }
        public readonly InvokableSignature signature { get; }

        public ConstructrorKey(Type type, InvokableSignature signature)
        {
            Type = type;
            this.signature = signature;
        }

        public bool Equals(ConstructrorKey other)
        {
            return other.Type == Type && other.signature == signature;
        }

        public override bool Equals(object? obj)
        {
            return obj is InvokableArguments typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, signature);
        }

        public static bool operator ==(ConstructrorKey left, ConstructrorKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConstructrorKey left, ConstructrorKey right)
        {
            return !left.Equals(right);
        }
    }
}
