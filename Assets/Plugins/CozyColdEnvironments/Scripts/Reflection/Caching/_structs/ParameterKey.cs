using CCEnvs.Reflection.Caching;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    [Serializable]
    public struct ParameterKey : IEquatable<ParameterKey>
    {
        private int? cachedHashCode;

        private string? cachedString;

        public MemberKey MemberPart { readonly get; init; }

        public bool ByRef { readonly get; init; }

        public ParameterKey(ParameterInfo param)
        {
            cachedHashCode = null;
            cachedString = null;

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


        public override string ToString()
        {
            if (this == default)
            {
                cachedString = StringHelper.EMPTY_OBJECT;

                return cachedString;
            }

            cachedString = $"({nameof(MemberPart)}: {MemberPart}; {nameof(ByRef)}: {ByRef})";

            return cachedString;
        }
    }
}
