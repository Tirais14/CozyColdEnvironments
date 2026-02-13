using CCEnvs.Caching;
using CCEnvs.Collections;
using CCEnvs.Reflection.Caching;
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
    public struct ReflectionFieldHandle : IEquatable<ReflectionFieldHandle>
    {
        private static readonly Cache<ReflectionFieldHandle, FieldKey> cachedFieldKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private int? hashCode;

        public ReflectionHandle BaseReflectionHandle { readonly get; init; }

        public Type? FieldTypeFilter { readonly get; init; }

        public ReflectionFieldHandle(ReflectionHandle baseReflectionHandle)
            :
            this()
        {
            BaseReflectionHandle = baseReflectionHandle; 
        }

        public static bool operator ==(ReflectionFieldHandle left, ReflectionFieldHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ReflectionFieldHandle left, ReflectionFieldHandle right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionFieldHandle WithBaseReflectionHandle(
            ReflectionHandle baseReflectionHandle
            )
        {
            return new ReflectionFieldHandle(baseReflectionHandle)
            {
                FieldTypeFilter = FieldTypeFilter,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionFieldHandle WithFieldTypeFilter(
            Type? fieldTypeFilter
            )
        {
            return new ReflectionFieldHandle(BaseReflectionHandle)
            {
                FieldTypeFilter = fieldTypeFilter
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReflectionFieldHandle WithFieldTypeFilter<T>()
        {
            return WithFieldTypeFilter(typeof(T));
        }

        public readonly IEnumerable<FieldInfo> GetFields()
        {
            return new FieldEnumerator(
                this,
                BaseReflectionHandle.GetMembers(MemberTypes.Field)
                );
        }

        public readonly FieldInfo? GetField(bool throwIfNotFound)
        {
            if (cachedFieldKeys.TryGet(this, out var fieldKey)
                &&
                CachedMembers.TryGetField(fieldKey, out var field)
                )
            {
                return field;
            }

            if (!GetFields().SingleOrDefault().Let(out field))
            {
                if (throwIfNotFound)
                    throw new InvalidOperationException($"Cannot find field by {this}");

                return null;
            }

            if (BaseReflectionHandle.CacheResults)
                CacheField(field);

            return field;

        }

        public readonly bool IsPropertyTypeMatch(Type fieldType)
        {
            if (FieldTypeFilter is null)
                return true;

            return fieldType.IsType(FieldTypeFilter);
        }

        public readonly bool Equals(ReflectionFieldHandle other)
        {
            return BaseReflectionHandle == other.BaseReflectionHandle
                   &&
                   FieldTypeFilter == other.FieldTypeFilter;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionFieldHandle typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(BaseReflectionHandle, FieldTypeFilter);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(BaseReflectionHandle)}: {BaseReflectionHandle}; {nameof(FieldTypeFilter)}: {FieldTypeFilter})";
        }

        private readonly void CacheField(FieldInfo field)
        {
            if (cachedFieldKeys.TryAdd(this, new FieldKey(field), out var entry))
                entry.ExpirationTimeRelativeToNow = BaseReflectionHandle.GetCacheExpirationTimeRelativeToNowOrDefault();

            CachedMembers.TryAddField(field);
        }

        public struct FieldEnumerator 
            :
            IEnumerator<FieldInfo?>,
            IEnumerable<FieldInfo>
        {
            private readonly ReflectionFieldHandle reflectionHandle;

            private readonly IEnumerable<MemberInfo> source;

            private IEnumerator<MemberInfo>? enumerator;

            public FieldEnumerator(
                ReflectionFieldHandle reflectionHandle,
                IEnumerable<MemberInfo> source
                ) 
                : 
                this()
            {
                this.reflectionHandle = reflectionHandle;
                this.source = source;
            }

            public FieldInfo? Current { readonly get; private set; }

            readonly object IEnumerator.Current => Current!;

            public bool MoveNext()
            {
                enumerator ??= source.GetEnumerator();

                while (enumerator.TryMoveNext(out var t))
                {
                    if (t is not FieldInfo field)
                        continue;

                    if (!reflectionHandle.IsFieldTypeMatch(field.FieldType))
                        continue;

                    Current = field;

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

            public readonly IEnumerator<FieldInfo> GetEnumerator()
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
