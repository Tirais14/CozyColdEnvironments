using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public struct FieldKey : IEquatable<FieldKey>
    {
        private int? hashCode;

        public MemberKey Core { readonly get; init; }

        public Type? FieldType { readonly get; init; }

        public bool IsInitOnly { readonly get; init; }

        public FieldKey(FieldInfo field)
        {
            hashCode = null;

            Core = field;
            FieldType = field.FieldType;
            IsInitOnly = field.IsInitOnly;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator FieldKey(FieldInfo field)
        {
            return new FieldKey(field);
        }

        public static bool operator ==(FieldKey left, FieldKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FieldKey left, FieldKey right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly FieldKey WithCore(MemberKey core)
        {
            return new FieldKey
            {
                Core = core,
                FieldType = FieldType,
                IsInitOnly = IsInitOnly
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly FieldKey WithFieldType(Type fieldType)
        {
            return new FieldKey
            {
                Core = Core,
                FieldType = fieldType,
                IsInitOnly = IsInitOnly
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly FieldKey WithIsInitOnly(bool isInitOnly)
        {
            return new FieldKey
            {
                Core = Core,
                FieldType = FieldType,
                IsInitOnly = isInitOnly
            };
        }

        public readonly bool Equals(FieldKey other)
        {
            return Core == other.Core
                   &&
                   FieldType == other.FieldType
                   &&
                   IsInitOnly == other.IsInitOnly;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is FieldKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Core, FieldType, IsInitOnly);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Core)}: {Core}; {nameof(FieldType)}: {FieldType}; {nameof(IsInitOnly)}: {IsInitOnly})";
        }
    }
}
