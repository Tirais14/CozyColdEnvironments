using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        private int? hashCode;

        public MemberKey Core { readonly get; init; }

        public Type? PropertyType { readonly get; init; }

        public bool CanRead { readonly get; init; }

        public bool CanWrite { readonly get; init; }

        public PropertyKey(PropertyInfo prop)
        {
            hashCode = null;

            Core = prop;
            PropertyType = prop.PropertyType;
            CanRead = prop.CanRead;
            CanWrite = prop.CanWrite;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator PropertyKey(PropertyInfo prop)
        {
            return new PropertyKey(prop);
        }

        public static bool operator ==(PropertyKey left, PropertyKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PropertyKey left, PropertyKey right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PropertyKey WithCore(MemberKey core)
        {
            return new PropertyKey
            {
                Core = core,
                PropertyType = PropertyType,
                CanRead = CanRead,
                CanWrite = CanWrite
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PropertyKey WithPropertyType(Type? propType)
        {
            return new PropertyKey
            {
                Core = Core,
                PropertyType = propType,
                CanRead = CanRead,
                CanWrite = CanWrite
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PropertyKey WithCanRead(bool canRead)
        {
            return new PropertyKey
            {
                Core = Core,
                PropertyType = PropertyType,
                CanRead = canRead,
                CanWrite = CanWrite
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PropertyKey WithCanWrite(bool canWrite)
        {
            return new PropertyKey
            {
                Core = Core,
                PropertyType = PropertyType,
                CanRead = CanRead,
                CanWrite = canWrite
            };
        }

        public readonly bool Equals(PropertyKey other)
        {
            return Core.Equals(other.Core)
                   &&
                   PropertyType == other.PropertyType
                   &&
                   CanRead == other.CanRead
                   &&
                   CanWrite == other.CanWrite;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is PropertyKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Core, PropertyType, CanRead, CanWrite);

            return hashCode.Value;
        }

        public readonly override string ToString()
        { 
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Core)}: {Core}; {nameof(PropertyType)}: {PropertyType}; {nameof(CanRead)}: {CanRead}; {nameof(CanWrite)}: {CanWrite})";
        }
    }
}
