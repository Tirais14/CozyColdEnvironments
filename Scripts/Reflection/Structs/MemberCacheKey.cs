using System;
using UTIRLib.Reflection;

#nullable enable
namespace UTIRLib
{
    public readonly struct MemberCacheKey : IEquatable<MemberCacheKey>
    {
        public readonly Type Type { get; }
        public readonly TypeMemberParameters MemberParameters { get; }

        public MemberCacheKey(Type type, TypeMemberParameters memberParameters)
        {
            Type = type;
            MemberParameters = memberParameters;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not MemberCacheKey other)
                return false;

            return Equals(other);
        }

        public bool Equals(MemberCacheKey other)
        {
            return other.Type == Type && other.MemberParameters == MemberParameters;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, MemberParameters);
        }
    }
}
