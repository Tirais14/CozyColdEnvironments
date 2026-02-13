#nullable enable
using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CCEnvs.Reflection.Caching
{
    [Serializable]
    public struct MethodKey : IEquatable<MethodKey>
    {
        private int? hashCode;

        public MemberKey Core { readonly get; init; }

        public StructuralArray<ParameterKey> ParameterKeys { readonly get; init; }

        public MethodKey(MethodBase method)
        {
            hashCode = null;

            Core = method;
            ParameterKeys = method.GetParameters()
                .ToParamteterKeys()
                .ToStructuralArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator MethodKey(MethodBase method)
        {
            return new MethodKey(method);
        }

        public static bool operator ==(MethodKey left, MethodKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MethodKey left, MethodKey right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithMemberPart(MemberKey memberPart)
        {
            CC.Guard.IsNotDefault(memberPart, nameof(memberPart));

            return new MethodKey
            {
                Core = memberPart,
                ParameterKeys = ParameterKeys
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithParameterKeys(params ParameterKey[] parameterKeys)
        {
            Guard.IsNotNull(parameterKeys, nameof(parameterKeys));

            return new MethodKey
            {
                Core = Core,
                ParameterKeys = parameterKeys.ToStructuralArray()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithParameterKeys(IEnumerable<ParameterKey> parameterKeys)
        {
            Guard.IsNotNull(parameterKeys, nameof(parameterKeys));

            return new MethodKey
            {
                Core = Core,
                ParameterKeys = parameterKeys.ToStructuralArray()
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithParameterKeys(StructuralArray<ParameterKey> parameterKeys)
        {
            Guard.IsNotNull(parameterKeys, nameof(parameterKeys));

            return new MethodKey
            {
                Core = Core,
                ParameterKeys = parameterKeys,
            };
        }

        public readonly bool Equals(MethodKey other)
        {
            return Core == other.Core
                   &&
                   ParameterKeys.EqualsByElements(other.ParameterKeys);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MethodKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Core, ParameterKeys);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Core)}: {Core})";
        }
    }
}
