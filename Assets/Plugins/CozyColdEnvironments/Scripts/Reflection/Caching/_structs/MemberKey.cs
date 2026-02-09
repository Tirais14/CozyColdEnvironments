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

        private string? cachedString;

        public string Name { readonly get; init; }
        
        public Type DeclaringType { readonly get; init; }

        public Type? UnderlyingType { readonly get; init; }

        public MemberTypes MemberType { readonly get; init; }

        public MemberKey(MemberInfo member)
        {
            cachedHashCode = null;
            cachedString = null;

            Name = member.Name;
            DeclaringType = member.DeclaringType;
            UnderlyingType = null;
            MemberType = member.MemberType;
        }

        public MemberKey(FieldInfo field)
            :
            this((MemberInfo)field)
        {
            UnderlyingType = field.FieldType;
        }

        public MemberKey(PropertyInfo prop)
            :
            this((MemberInfo)prop)
        {
            UnderlyingType = prop.PropertyType;
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
        public static implicit operator MemberKey(FieldInfo field)
        {
            return new MemberKey(field);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator MemberKey(PropertyInfo prop)
        {
            return new MemberKey(prop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithName(string name)
        {
            Guard.IsNotNull(name, nameof(name));

            return new MemberKey
            {
                Name = name,
                DeclaringType = DeclaringType,
                UnderlyingType = UnderlyingType,
                MemberType = MemberType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithDeclaringType(Type declaringType)
        {
            Guard.IsNotNull(declaringType, nameof(declaringType));

            return new MemberKey
            {
                Name = Name,
                DeclaringType = declaringType,
                UnderlyingType = UnderlyingType,
                MemberType = MemberType
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey WithUnderlyingType(Type? underlyingType)
        {
            Guard.IsNotNull(underlyingType, nameof(underlyingType));

            return new MemberKey
            {
                Name = Name,
                DeclaringType = DeclaringType,
                UnderlyingType = underlyingType,
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
                UnderlyingType = UnderlyingType,
                MemberType = memberType
            };
        }

        public readonly bool Equals(MemberKey other)
        {
            return Name == other.Name
                   &&
                   DeclaringType == other.DeclaringType
                   &&
                   UnderlyingType == other.UnderlyingType
                   &&
                   MemberType == other.MemberType;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MemberKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            cachedHashCode ??= HashCode.Combine(Name, DeclaringType, UnderlyingType, MemberType);

            return cachedHashCode.Value;
        }

        public override string ToString()
        {
            if (this == default)
            {
                cachedString = StringHelper.EMPTY_OBJECT;

                return cachedString;
            }

            cachedString = $"({nameof(Name)}: {Name}; {nameof(MemberType)}: {MemberType}; {nameof(DeclaringType)}: {DeclaringType})";

            return cachedString;
        }
    }
}
