using CommunityToolkit.Diagnostics;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    [Serializable]
    public struct MemberKey : IEquatable<MemberKey>
    {
        private int? cachedHashCode;

        public string? Name { readonly get; init; }

        public Type? DeclaringType { readonly get; init; }

        public MemberTypes MemberType { readonly get; init; }

        public MemberKey(MemberInfo member)
        {
            cachedHashCode = null;

            Name = member.Name;
            DeclaringType = member.DeclaringType;
            MemberType = member.MemberType;
        }

        public static bool operator ==(MemberKey left, MemberKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemberKey left, MemberKey right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator MemberKey(MemberInfo member)
        {
            return new MemberKey(member);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithName(string? name)
        {
            return new MemberKey
            {
                Name = name,
                DeclaringType = DeclaringType,
                MemberType = MemberType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithDeclaringType(Type? declaringType)
        {
            Guard.IsNotNull(declaringType, nameof(declaringType));

            return new MemberKey
            {
                Name = Name,
                DeclaringType = declaringType,
                MemberType = MemberType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithMemberType(MemberTypes memberType)
        {
            return new MemberKey
            {
                Name = Name,
                DeclaringType = DeclaringType,
                MemberType = memberType
            };
        }

        public readonly bool Equals(MemberKey other)
        {
            return Name == other.Name
                   &&
                   DeclaringType == other.DeclaringType
                   &&
                   MemberType == other.MemberType;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MemberKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            cachedHashCode ??= HashCode.Combine(Name, DeclaringType, MemberType);

            return cachedHashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Name)}: {Name}; {nameof(MemberType)}: {MemberType}; {nameof(DeclaringType)}: {DeclaringType})";
        }
    }
}
