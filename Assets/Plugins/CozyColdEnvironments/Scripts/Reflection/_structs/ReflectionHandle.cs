using CCEnvs.Caching;
using CCEnvs.Linq;
using CCEnvs.Reflection.Caching;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionHandle : IEquatable<ReflectionHandle>
    {
        public static ReflectionHandle Instance { get; } = new();

        private static readonly Cache<ReflectionHandle, MemberKey> cachedMemberKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes(),
            SizeLimit = 163840
        };

        private static readonly Cache<ReflectionHandle, MemberInfo[]> cachedMembers = new()
        {
            ExpirationScanFrequency = 1.Minutes(),
            SizeLimit = 163840
        };

        private int? hashCode;

        public Type? Type { readonly get; init; }

        public BindingFlags Bindings { readonly get; init; }

        public StringMatchSettings StringMatchOptions { readonly get; init; }

        public string? NameFilter { readonly get; init; }

        public bool IsCacheResults { readonly get; init; }

        public TimeSpan? ExpirationTimeRelativeToNow { readonly get; init; }

        public static bool operator ==(ReflectionHandle left, ReflectionHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectionHandle left, ReflectionHandle right)
        {
            return !(left == right);
        }

        public static ReflectionHandle Create()
        {
            return new ReflectionHandle
            {
                Bindings = BindingFlags.Default,
                StringMatchOptions = StringMatchSettings.Default,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithType(Type? type = null)
        {
            return new ReflectionHandle
            {
                Type = type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                IsCacheResults = IsCacheResults,
                ExpirationTimeRelativeToNow = ExpirationTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithBindings(BindingFlags bindings = BindingFlags.Default)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                IsCacheResults = IsCacheResults,
                ExpirationTimeRelativeToNow = ExpirationTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithStringMatchSettings(StringMatchSettings stringMatchSettings = StringMatchSettings.Default)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = stringMatchSettings,
                NameFilter = NameFilter,
                IsCacheResults = IsCacheResults,
                ExpirationTimeRelativeToNow = ExpirationTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithNameFilter(string? filter)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = filter,
                IsCacheResults = IsCacheResults,
                ExpirationTimeRelativeToNow = ExpirationTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle CacheResults(bool state = true)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                IsCacheResults = state,
                ExpirationTimeRelativeToNow = ExpirationTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle CacheResultsWithExpirationTimeRelativeToNow(TimeSpan? cacheExpiratiomTimeRelativeToNow)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                IsCacheResults = IsCacheResults,
                ExpirationTimeRelativeToNow = cacheExpiratiomTimeRelativeToNow ?? 20.Minutes(),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsNameMatch(string name)
        {
            if (NameFilter.IsNullOrWhiteSpace())
                return true;

            return NameFilter.Match(name, StringMatchOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionMethodHandle ContinueWithMethods()
        {
            return new ReflectionMethodHandle(this);
        }

        public readonly MemberInfo[] GetMembers(MemberTypes memberType)
        {
            Guard.IsNotNull(Type, nameof(Type));

            if (cachedMembers.TryGetValue(this, out var members))
                return members;

            members = Type.FindMembers(memberType, Bindings,
                static (member, state) =>
                {
                    var stateTyped = (ReflectionHandle)state;

                    if (!stateTyped.IsNameMatch(member.Name))
                        return false;

                    return true;
                },
                this
                );

            if (IsCacheResults)
                CacheMembers(members);

            return members;
        }

        public readonly MemberInfo? GetMember(
            MemberTypes memberType,
            bool throwIfNotFound = false
            )
        {
            if (cachedMemberKeys.TryGetValue(this, out var memberKey)
                &&
                CachedMembers.TryGetMemberUntyped(memberKey, memberType, out var member))
            {
                return member;
            }

            if (GetMembers(memberType).SingleOrDefault().IsNot(out member))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find any member by {this}");

                return null;
            }

            if (IsCacheResults)
                CacheMember(member);

            return member;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TimeSpan GetCacheExpirationTimeRelativeToNowOrDefault()
        {
            return ExpirationTimeRelativeToNow ?? 20.Minutes();
        }

        public readonly bool Equals(ReflectionHandle other)
        {
            return Type == other.Type
                   &&
                   Bindings == other.Bindings
                   &&
                   StringMatchOptions == other.StringMatchOptions
                   &&
                   NameFilter == other.NameFilter
                   &&
                   IsCacheResults == other.IsCacheResults
                   &&
                   ExpirationTimeRelativeToNow == other.ExpirationTimeRelativeToNow;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionHandle typed && Equals(typed);
        }

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Type), Type)
                .AddProperty(nameof(Bindings), Bindings)
                .AddProperty(nameof(StringMatchOptions), StringMatchOptions)
                .AddProperty(nameof(NameFilter), NameFilter)
                .AddProperty(nameof(IsCacheResults), IsCacheResults)
                .AddProperty(nameof(ExpirationTimeRelativeToNow), ExpirationTimeRelativeToNow)
                .ToStringAndDispose();
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(
                Type,
                Bindings,
                StringMatchOptions,
                NameFilter,
                IsCacheResults,
                ExpirationTimeRelativeToNow
                );

            return hashCode.Value;
        }

        private readonly void CacheMember(MemberInfo member)
        {
            if (cachedMemberKeys.TryAdd(this, new MemberKey(member), out var entry))
                entry.ExpirationTimeRelativeToNow = GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddMemberUntyped(member, out _, GetCacheExpirationTimeRelativeToNowOrDefault());
        }

        private readonly void CacheMembers(MemberInfo[] members)
        {
            if (cachedMembers.TryAdd(this, members, out var entry))
                entry.ExpirationTimeRelativeToNow = GetCacheExpirationTimeRelativeToNowOrDefault();

            for (int i = 0; i < members.Length; i++)
                CachedMembers.TryAddMemberUntyped(members[i], out _, GetCacheExpirationTimeRelativeToNowOrDefault());
        }
    }

    public static class ReflectionHandleExtensions
    {
        public static ReflectionHandle GetReflectionHandle(this Type type)
        {
            Guard.IsNotNull(type);
            return new ReflectionHandle().WithType(type);
        }

        public static ReflectionHandle GetReflectionHandle(this object obj)
        {
            Guard.IsNotNull(obj);
            return new ReflectionHandle().WithType(obj.GetType());
        }
    }
}
