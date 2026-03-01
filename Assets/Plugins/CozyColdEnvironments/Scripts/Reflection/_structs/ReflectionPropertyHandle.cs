using CCEnvs.Caching;
using CCEnvs.Collections;
using CCEnvs.Reflection.Caching;
using CCEnvs.TypeMatching;
using Humanizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionPropertyHandle : IEquatable<ReflectionPropertyHandle>
    {
        private static readonly Cache<ReflectionPropertyHandle, PropertyKey> cachedPropKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private int? hashCode;

        public ReflectionHandle BaseReflectionHandle { readonly get; init; }

        public Type? PropertyTypeFilter { readonly get; init; }

        public ReflectionPropertyHandle(ReflectionHandle baseReflectionHandle)
            :
            this()
        {
            BaseReflectionHandle = baseReflectionHandle;
        }

        public static bool operator ==(ReflectionPropertyHandle left, ReflectionPropertyHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectionPropertyHandle left, ReflectionPropertyHandle right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionPropertyHandle WithBaseReflectionHandle(
            ReflectionHandle baseReflectionHandle
            )
        {
            return new ReflectionPropertyHandle(baseReflectionHandle)
            {
                PropertyTypeFilter = PropertyTypeFilter,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionPropertyHandle WithPropertyTypeFilter(
            Type? fieldTypeFilter
            )
        {
            return new ReflectionPropertyHandle(BaseReflectionHandle)
            {
                PropertyTypeFilter = fieldTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionPropertyHandle WithPropertyTypeFilter<T>()
        {
            return WithPropertyTypeFilter(typeof(T));
        }

        public readonly IEnumerable<PropertyInfo> GetProperties()
        {
            return new PropertyEnumerator(
                this,
                BaseReflectionHandle.GetMembers(MemberTypes.Property)
                );
        }

        public readonly PropertyInfo? GetProperty(bool throwIfNotFound)
        {
            if (cachedPropKeys.TryGet(this, out var propKey)
                &&
                CachedMembers.TryGetProperty(propKey, out var prop)
                )
            {
                return prop;
            }

            if (GetProperties().SingleOrDefault().IsNot(out prop))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find property by {nameof(ReflectionFieldHandle)}: {this}");

                return null;
            }

            if (BaseReflectionHandle.CacheResults)
                CacheProperty(prop);

            return prop;

        }

        public readonly bool IsFieldTypeMatch(Type fieldType)
        {
            if (PropertyTypeFilter is null)
                return true;

            return fieldType.IsType(PropertyTypeFilter);
        }

        public readonly bool Equals(ReflectionPropertyHandle other)
        {
            return BaseReflectionHandle == other.BaseReflectionHandle
                   &&
                   PropertyTypeFilter == other.PropertyTypeFilter;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionPropertyHandle typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(BaseReflectionHandle, PropertyTypeFilter);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(BaseReflectionHandle)}: {BaseReflectionHandle}; {nameof(PropertyTypeFilter)}: {PropertyTypeFilter})";
        }

        private readonly void CacheProperty(PropertyInfo prop)
        {
            if (cachedPropKeys.TryAdd(this, new PropertyKey(prop), out var entry))
                entry.ExpirationTimeRelativeToNow = BaseReflectionHandle.GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddProperty(prop);
        }

        public struct PropertyEnumerator
            :
            IEnumerator<PropertyInfo?>,
            IEnumerable<PropertyInfo>
        {
            private readonly ReflectionPropertyHandle reflectionHandle;

            private readonly IEnumerable<MemberInfo> source;

            private IEnumerator<MemberInfo>? enumerator;

            public PropertyEnumerator(
                ReflectionPropertyHandle reflectionHandle,
                IEnumerable<MemberInfo> source
                )
                :
                this()
            {
                this.reflectionHandle = reflectionHandle;
                this.source = source;
            }

            public PropertyInfo? Current { readonly get; private set; }

            readonly object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                enumerator ??= source.GetEnumerator();

                while (enumerator.TryMoveNext(out var t))
                {
                    if (t is not PropertyInfo prop)
                        continue;

                    if (!reflectionHandle.IsFieldTypeMatch(prop.PropertyType))
                        continue;

                    Current = prop;

                    return true;
                }

                return false;
            }

            public readonly void Reset()
            {
                throw new NotSupportedException(nameof(Reset));
            }

            public readonly void Dispose()
            {
            }

            public readonly IEnumerator<PropertyInfo> GetEnumerator()
            {
                return this;
            }

            readonly IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
