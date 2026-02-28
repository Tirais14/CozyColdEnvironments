using CCEnvs.Reflection.Caching;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    [Serializable]
    public struct ParameterKey : IEquatable<ParameterKey>
    {
        private int? cachedHashCode;

        public MemberKey MemberPart { readonly get; init; }

        public bool ByRef { readonly get; init; }

        public ParameterKey(ParameterInfo param)
        {
            cachedHashCode = null;

            MemberPart = new MemberKey(param.Member);

            ByRef = param.ParameterType.IsByRef;
        }

        public static bool operator ==(ParameterKey left, ParameterKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ParameterKey left, ParameterKey right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ParameterKey(ParameterInfo param)
        {
            return new ParameterKey(param);
        }

        public readonly bool Equals(ParameterKey other)
        {
            return MemberPart == other.MemberPart
                   &&
                   ByRef == other.ByRef;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is ParameterKey key && Equals(key);
        }

        public override int GetHashCode()
        {
            cachedHashCode ??= HashCode.Combine(MemberPart, ByRef);

            return cachedHashCode.Value;
        }


        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(MemberPart)}: {MemberPart}; {nameof(ByRef)}: {ByRef})";
        }
    }

    public static class ParameterKeyExtensions
    {
        public static ParameterModifier ToParameterModifiers(
            this ICollection<ParameterKey> source
            )
        {
            CC.Guard.IsNotNullSource(source);

            var mods = new ParameterModifier(source.Count);

            int i = 0;

            foreach (var paramKey in source)
                mods[i++] = paramKey.ByRef;

            return mods;
        }

        public static ParameterKey ToParameterKey(this ParameterInfo paramInfo)
        {
            return new ParameterKey(paramInfo);
        }
    }
}
