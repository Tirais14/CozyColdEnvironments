#nullable enable
using CCEnvs.Collections;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace CCEnvs.Reflection.Caching
{
    [Serializable]
    public struct MethodKey : IEquatable<MethodKey>
    {
        private int? cachedHashCode;

        private string? cachedString;

        public MemberKey MemberPart { readonly get; init; }

        public ImmutableArray<ParameterKey> ParameterKeys { readonly get; init; }

        public MethodKey(MethodBase method)
        {
            cachedHashCode = null;
            cachedString = null;

            MemberPart = method;

            var paramKeys = ListPool<ParameterKey>.Shared.Get();

            foreach (var param in method.GetParameters())
                paramKeys.Value.Add(new ParameterKey(param));

            ParameterKeys = paramKeys.Value.ToImmutableArray();
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
        public static implicit operator MethodKey(MethodBase method)
        {
            return new MethodKey(method); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithMemberPart(MemberKey memberPart)
        {
            CC.Guard.IsNotDefault(memberPart, nameof(memberPart));

            return new MethodKey
            {
                MemberPart = memberPart,
                ParameterKeys = ParameterKeys
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MethodKey WithParameterKeys(params ParameterKey[] parameterKeys)
        {
            Guard.IsNotNull(parameterKeys, nameof(parameterKeys));

            return new MethodKey
            {
                MemberPart = MemberPart,
                ParameterKeys = parameterKeys.ToImmutableArray()
            };
        }

        public readonly bool Equals(MethodKey other)
        {
            return MemberPart == other.MemberPart
                   &&
                   ParameterKeys.EqualsByElements(other.ParameterKeys);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is MethodKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            cachedHashCode = HashCode.Combine(MemberPart, ParameterKeys.HashCodeByElements());

            return cachedHashCode.Value;
        }

        public override string ToString()
        {
            if (this == default)
            {
                cachedString = StringHelper.EMPTY_OBJECT;

                return cachedString;
            }

            cachedString = $"({nameof(MemberPart)}: {MemberPart})";

            return cachedString;
        }
    }
}
