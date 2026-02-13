using CCEnvs.Caching;
using CCEnvs.Reflection.Caching;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Reflection
{
    public struct ReflectionFieldHandle : IEquatable<ReflectionFieldHandle>
    {
        private static readonly Cache<ReflectionFieldHandle, FieldKey> fieldKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private static readonly Cache<ReflectionFieldHandle, PropertyKey> propKeys = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        private int? hashCode;

        public ReflectionHandle BaseReflectionHandle { readonly get; init; }

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
            return new ReflectionFieldHandle(baseReflectionHandle);
        }

        public IEnumerable<FieldInfo> GetFields()
        {

        }

        public FieldInfo? GetField(bool throwIfNotFound)
        {
            if (fieldKeys.TryGet(this, out var fieldKey)
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

        public readonly bool Equals(ReflectionFieldHandle other)
        {
            return BaseReflectionHandle == other.BaseReflectionHandle;
        }

        public readonly override bool Equals(object obj)
        {
            return obj is ReflectionFieldHandle typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            hashCode ??= HashCode.Combine(BaseReflectionHandle);

            return hashCode.Value;
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(BaseReflectionHandle)}: {BaseReflectionHandle})";
        }

        private readonly void CacheField(FieldInfo field)
        {
            if (fieldKeys.TryAdd(this, new FieldKey(field), out var entry))
                entry.ExpirationTimeRelativeToNow = 20.Minutes();

            CachedMembers.TryAddField(field);
        }
    }
}
