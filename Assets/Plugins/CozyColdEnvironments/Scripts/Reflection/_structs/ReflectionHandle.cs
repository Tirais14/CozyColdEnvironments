using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CCEnvs.Caching;
using CCEnvs.Linq;
using CCEnvs.Reflection.Caching;
using CCEnvs.TypeMatching;
using CommunityToolkit.Diagnostics;
using Humanizer;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionHandle : IEquatable<ReflectionHandle>
    {
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

        public bool CacheResults { readonly get; init; }

        public TimeSpan? CacheExpiratiomTimeRelativeToNow { readonly get; init; }

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
                CacheResults = CacheResults,
                CacheExpiratiomTimeRelativeToNow = CacheExpiratiomTimeRelativeToNow,
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
                CacheResults = CacheResults,
                CacheExpiratiomTimeRelativeToNow = CacheExpiratiomTimeRelativeToNow,
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
                CacheResults = CacheResults,
                CacheExpiratiomTimeRelativeToNow = CacheExpiratiomTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithNameFilter(string? name)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = name,
                CacheResults = CacheResults,
                CacheExpiratiomTimeRelativeToNow = CacheExpiratiomTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithCacheResults(bool state = true)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                CacheResults = state,
                CacheExpiratiomTimeRelativeToNow = CacheExpiratiomTimeRelativeToNow,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionHandle WithCacheExpirationTimeRelativeToNoew(TimeSpan? cacheExpiratiomTimeRelativeToNow)
        {
            return new ReflectionHandle
            {
                Type = Type,
                Bindings = Bindings,
                StringMatchOptions = StringMatchOptions,
                NameFilter = NameFilter,
                CacheResults = CacheResults,
                CacheExpiratiomTimeRelativeToNow = cacheExpiratiomTimeRelativeToNow ?? 20.Minutes(),
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

            if (CacheResults)
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

            if (CacheResults)
                CacheMember(member);

            return member;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly TimeSpan GetCacheExpirationTimeRelativeToNowOrDefault()
        {
            return CacheExpiratiomTimeRelativeToNow ?? 20.Minutes();
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
                   CacheResults == other.CacheResults;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionMethodHandle typed && Equals(typed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(Type)}: {Type}; {nameof(Bindings)}: {Bindings}; {nameof(StringMatchOptions)}: {StringMatchOptions}; {nameof(NameFilter)}: {NameFilter}; {nameof(CacheResults)}: {CacheResults})";
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(
                Type,
                Bindings,
                StringMatchOptions,
                NameFilter,
                CacheResults
                );

            return hashCode.Value;
        }

        private readonly void CacheMember(MemberInfo member)
        {
            if (cachedMemberKeys.TryAdd(this, new MemberKey(member), out var entry))
                entry.ExpirationTimeRelativeToNow = GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddMemberUntyped(member, GetCacheExpirationTimeRelativeToNowOrDefault());
        }

        private readonly void CacheMembers(MemberInfo[] members)
        {
            if (cachedMembers.TryAdd(this, members, out var entry))
                entry.ExpirationTimeRelativeToNow = GetCacheExpirationTimeRelativeToNowOrDefault();

            for (int i = 0; i < members.Length; i++)
                CachedMembers.TryAddMemberUntyped(members[i], GetCacheExpirationTimeRelativeToNowOrDefault());
        }
    }
}
