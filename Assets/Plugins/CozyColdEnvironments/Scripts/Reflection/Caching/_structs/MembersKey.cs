using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection.Caching
{
    public struct MembersKey : IEquatable<MembersKey>
    {
        private int? hashCode;

        public BindingFlags Bindings { get; init; }

        public Type? DeclaringType { get; init; }

        public MemberTypes MemberType { get; init; }

        public MembersKey(BindingFlags bindings, Type? declaringType, MemberTypes memberType)
        {
            hashCode = null;

            Bindings = bindings;
            DeclaringType = declaringType;
            MemberType = memberType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MembersKey WithBindings(BindingFlags bindings)
        {
            return new MembersKey(bindings, DeclaringType, MemberType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MembersKey WithDeclaringType(Type? declaringType)
        {
            return new MembersKey(Bindings, declaringType, MemberType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MembersKey WithMemberType(MemberTypes memberType)
        {
            return new MembersKey(Bindings, DeclaringType, memberType);
        }

        public readonly bool Equals(MembersKey other)
        {
            return Bindings == other.Bindings
                   &&
                   EqualityComparer<Type?>.Default.Equals(DeclaringType, other.DeclaringType)
                   &&
                   MemberType == other.MemberType;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is MembersKey typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(Bindings, DeclaringType, MemberType);

            return hashCode.Value;
        }
    }
}
