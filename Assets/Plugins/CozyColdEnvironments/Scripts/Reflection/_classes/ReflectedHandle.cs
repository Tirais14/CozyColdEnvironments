using CCEnvs.Reflection.Caching;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.SocialPlatforms.Impl;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectedHandle : IEquatable<ReflectedHandle>
    {
        private int? hashCode;

        public Type? Type { get; set; }

        public BindingFlags Bindings { get; set; }

        public StringMatchSettings StringMatchSettings { get; set; }

        public string? NameFilter { get; set; }

        public bool CacheResults { get; set; }

        public static bool operator ==(ReflectedHandle left, ReflectedHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectedHandle left, ReflectedHandle right)
        {
            return !(left == right);
        }

        public static ReflectedHandle Create()
        {
            return new ReflectedHandle
            {
                Bindings = BindingFlags.Default,
                StringMatchSettings = StringMatchSettings.Default,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectedHandle WithType(Type? type = null)
        {
            Type = type;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectedHandle WithBindings(BindingFlags bindings = BindingFlags.Default)
        {
            Bindings = bindings;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectedHandle WithBindings(StringMatchSettings stringMatchSettings = StringMatchSettings.Default)
        {
            StringMatchSettings = stringMatchSettings;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectedHandle WithNameFilter(string? name)
        {
            NameFilter = name;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReflectedHandle WithCacheResults(bool state = true)
        {
            CacheResults = state;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsNameMatch(string name)
        {
            if (NameFilter.IsNullOrWhiteSpace())
                return true;

            return NameFilter.Match(name, StringMatchSettings);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectedMethodHandle ForMethods()
        {
            return new ReflectedMethodHandle(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MembersKey GetMembersKey(MemberTypes memberType)
        {
            return new MembersKey(Bindings, Type, memberType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MemberKey GetMemberKey(MemberTypes memberType)
        {
            return new MemberKey
            {
                DeclaringType = Type,
                Name = NameFilter,
                MemberType = memberType
            };
        }

        public readonly MemberInfo[] GetMembers(MemberTypes memberType)
        {
            Guard.IsNotNull(Type, nameof(Type));

            var membersKey = GetMembersKey(memberType);

            if (CachedMembers.TryGetTypeMembers(membersKey, out var members))
                return members;

            members = memberType switch
            {
                MemberTypes.All => Type.GetMembers(Bindings),
                MemberTypes.Constructor => Type.GetConstructors(Bindings),
                MemberTypes.Custom => throw new NotImplementedException(),
                MemberTypes.Event => Type.GetEvents(Bindings),
                MemberTypes.Field => Type.GetFields(Bindings),
                MemberTypes.Method => Type.GetMethods(Bindings),
                MemberTypes.NestedType => Type.GetNestedTypes(Bindings),
                MemberTypes.Property => Type.GetProperties(Bindings),
                MemberTypes.TypeInfo => throw new NotImplementedException(),
                _ => throw new InvalidOperationException(memberType.ToString())
            };

            if (CacheResults)
                CachedMembers.TryAddTypeMembers(membersKey, members);

            return members;
        }

        public readonly bool Equals(ReflectedHandle other)
        {
            return Type == other.Type
                   &&
                   Bindings == other.Bindings
                   &&
                   StringMatchSettings == other.StringMatchSettings
                   &&
                   NameFilter == other.NameFilter
                   &&
                   CacheResults == other.CacheResults;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectedMethodHandle typed && Equals(typed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Type)}: {Type}; {nameof(Bindings)}: {Bindings}; {nameof(StringMatchSettings)}: {StringMatchSettings}; {nameof(NameFilter)}: {NameFilter}; {nameof(CacheResults)}: {CacheResults})";
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(
                Type,
                Bindings,
                StringMatchSettings,
                NameFilter,
                CacheResults
                );

            return hashCode.Value;
        }
    }
}
